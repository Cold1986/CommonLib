using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cold.CommonLibrary
{
    class SpringObjectFactory
    {
        private IDictionary<string, object> objectDefine = new Dictionary<string, object>();

        private SpringObjectFactory(string fileName)
        {
            InstanceObjects(fileName);  // 实例IoC容器
            DiObjects(fileName);  // 属性注入
        }

        private static SpringObjectFactory instance;

        private static object lockHelper = new object();

        public static SpringObjectFactory Instance(string fileName)
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    instance = instance ?? new SpringObjectFactory(fileName);
                }
            }
            return instance;
        }

        /// <summary>
        /// 实例IoC容器
        /// </summary>
        /// <param name="fileName"></param>
        private void InstanceObjects(string fileName)
        {
            XElement root = XElement.Load(fileName);
            var objects = from obj in root.Elements("object")
                          select obj;

            //无参构造函数
            objectDefine = objects.Where(obj =>
                obj.Element("constructor-arg") == null).ToDictionary(
                    k => k.Attribute("id").Value,
                    v =>
                    {
                        string typeName = v.Attribute("type").Value;
                        Type type = Type.GetType(typeName);
                        return Activator.CreateInstance(type);
                    }
                );

            //有参构造函数
            foreach (XElement item in objects.Where(obj =>
                obj.Element("constructor-arg") != null))
            {
                string id = item.Attribute("id").Value;
                string typeName = item.Attribute("type").Value;
                Type type = Type.GetType(typeName);
                var args = from property in type.GetConstructors()[0].GetParameters()
                           join el in item.Elements("constructor-arg")
                           on property.Name equals el.Attribute("name").Value
                           select Convert.ChangeType(el.Attribute("value").Value,
                           property.ParameterType);
                object obj = Activator.CreateInstance(type, args.ToArray());
                objectDefine.Add(id, obj);
            }
        }

        /// <summary>
        /// 属性注入
        /// </summary>
        /// <param name="fileName"></param>
        private void DiObjects(string fileName)
        {
            XElement root = XElement.Load(fileName);
            var objects = from obj in root.Elements("object")
                          select obj;

            foreach (KeyValuePair<string, object> item in objectDefine)
            {
                foreach (var el in objects.Where(e =>
                    e.Attribute("id").Value == item.Key).Elements("property"))
                {
                    Type type = item.Value.GetType();
                    //获取属性
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (property.Name == el.Attribute("name").Value)
                        {
                            if (el.Attribute("value") != null)
                            {
                                //设置属性值
                                property.SetValue(item.Value,
                                    Convert.ChangeType(el.Attribute("value").Value,
                                    property.PropertyType), null);
                            }
                            else if (el.Attribute("ref") != null)
                            {
                                object refObject = null;

                                if (objectDefine.ContainsKey(el.Attribute("ref").Value))
                                {
                                    refObject = objectDefine[el.Attribute("ref").Value];
                                }
                                //设置关联对象属性
                                property.SetValue(item.Value, refObject, null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetObject(string id)
        {
            object result = null;

            if (objectDefine.ContainsKey(id))
            {
                result = objectDefine[id];
            }

            return result;
        }
    }
}
