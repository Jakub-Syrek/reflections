using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wf_Reflections
{

    public static class Reflect
    {

        public static List<string> vs = new List<string>();
        public static String val = "test";
        //public Container[] Container = new Container[1];
        public static bool hasFields(Type type)
        {
           var res = type.GetRuntimeFields() ?? type.GetFields(); 
           return res.Any();
        }
        public static bool CheckIfTypeFieldHasField(Type type, string fieldName)
        {
            var x = type.GetField(fieldName);
            if (x is IEnumerable)
            {
                return true;
            }
            return false;
        }

        public static bool CheckIfValueType<T>(T type) => type is ValueType;
        public static bool CheckIfValueType1<T>(T type) => typeof(T).IsValueType;
        public static bool CheckIfAbstractType<T>(T type) => typeof(T).IsAbstract;
        public static bool CheckIfPrimitiveType<T>(T type) => typeof(T).IsPrimitive;
        public static dynamic SplitChildren(object o)
        {
            if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
            {
                foreach (var item in o as IEnumerable)
                    return item;
            }
            return null;
        }
        public static bool IsSplittable(object o)
        {

            var x = o.GetType().Name;
            if (x == "String")
            {
                return false;
            }
            
            if (x.Contains("List"))
            {
                return true;
            }
            if (typeof(IEnumerable).IsAssignableFrom(o.GetType()))
            {
                foreach (var item in o as IEnumerable)
                    return true;
            }
            if (x.Contains("IEnumerable"))
            {
                return true;
            }
            return false;
        }
        public static dynamic ReturnChildren(object o)
        {
            if (CheckIfAbstractType(o.GetType()))
            {
                try
                {
                    return SplitChildren(o);
                }
                catch (Exception)
                {
                    return o;
                }
            }
            if (CheckIfValueType(o.GetType()) || CheckIfPrimitiveType(o.GetType()))
                return o;
            else
            {
                var t = o.GetType();
                if (t.Name == "String")
                    return o;

                var res = t.GetRuntimeFields();
                foreach (var item in res)
                {
                    var x = item;
                }
                return res;
            }
        }
        public static TreeNode buildTreeNode(object o)
        {
            object tmp = o;
            TreeNode rootTreeNode = new TreeNode();

            //Is a IENUMERABLE?
            if (IsSplittable(tmp))
            {
                foreach (object obj in tmp as IEnumerable)
                {
                    TreeNode subTreeNode = new TreeNode(obj.GetType().Name);

                    //Is a IENUMERABLE?                       
                    if (IsSplittable(obj))
                    {
                        foreach (object ob in obj as IEnumerable)
                        {
                            TreeNode subSubTreeNode = new TreeNode(ob.GetType().Name);
                            //Is a IENUMERABLE?
                            if (IsSplittable(ob))
                            {
                                foreach (object b in ob as IEnumerable)
                                {
                                    TreeNode subSubSubTreeNode = new TreeNode(b.GetType().Name);
                                    subSubTreeNode.Nodes.Add(subSubTreeNode);
                                }
                            }
                            else
                            {
                                if (hasFields(ob.GetType()))
                                {
                                    var obFields = (ob.GetType() as Type).GetRuntimeFields();

                                    foreach (FieldInfo item in obFields)
                                    {
                                        if (item.FieldType.Name.Contains("Enum"))
                                        {
                                            subSubTreeNode.Nodes.Add($"{item} as enum");
                                            continue;
                                            //build a final node
                                        }
                                        if (item.FieldType.Name == "String")
                                        {
                                            subSubTreeNode.Nodes.Add(item.GetValue(item).ToString());
                                            continue;
                                            //build a final node
                                        }
                                        else if (CheckIfValueType(item.FieldType))
                                        {
                                            subSubTreeNode.Nodes.Add(item.GetValue(item).ToString());
                                            continue;
                                            //build a final node
                                        }
                                        else if (item.FieldType.GetRuntimeFields() != null)
                                        {
                                            //build a final multi node
                                            var ClassFields = (item.FieldType as Type).GetRuntimeFields();
                                            foreach (FieldInfo fi in ClassFields)
                                            {
                                                if (fi.FieldType.FullName is string)
                                                {
                                                    subSubTreeNode.Nodes.Add(fi.Name);
                                                    continue;
                                                    //build a final node
                                                    //add to multi node
                                                }
                                                else if (CheckIfValueType(fi.FieldType))
                                                {
                                                    subSubTreeNode.Nodes.Add(fi.Name);
                                                    continue;
                                                    //build a final node
                                                    //add to multi node
                                                }
                                                else if (fi.FieldType.IsClass)
                                                {
                                                    var x = 1;
                                                    //build a final multi node

                                                    //var ClassFields = (fi.FieldType as Type).GetRuntimeFields();
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            subTreeNode.Nodes.Add(subSubTreeNode);
                        }
                    }
                    else
                    {
                        if (hasFields(obj.GetType()))
                        {
                            var obFields = (obj.GetType() as Type).GetRuntimeFields();

                            foreach (FieldInfo item in obFields)
                            {
                                
                                if (item.FieldType.Name == "String")
                                {
                                    subTreeNode.Nodes.Add(item.ToString());
                                    continue;
                                    //build a final node
                                }

                                else if(IsSplittable(item))
                                {
                                    foreach (object ob in item as IEnumerable)
                                    {
                                        TreeNode subSubTreeNode = new TreeNode(ob.GetType().Name);
                                        //Is a IENUMERABLE?
                                        if (IsSplittable(ob))
                                        {
                                            foreach (object b in ob as IEnumerable)
                                            {
                                                TreeNode subSubSubTreeNode = new TreeNode(b.GetType().Name);
                                                subSubTreeNode.Nodes.Add(subSubSubTreeNode);
                                            }
                                        }
                                        else
                                        {
                                            if (hasFields(ob.GetType()))
                                            {
                                                var obbFields = (ob.GetType() as Type).GetRuntimeFields();
                                            }
                                        }
                                    }
                                    subTreeNode.Nodes.Add(item.ToString());
                                    continue;
                                }

                                else if(item.FieldType.Name.Contains("Enum") && (item.FieldType.Name.Contains("Enumerable")==false))
                                {
                                    subTreeNode.Nodes.Add($"{item} as enum");
                                    continue;
                                    //build a final node
                                }


                                else if (CheckIfValueType(item.FieldType))
                                {
                                    subTreeNode.Nodes.Add(item.GetValue(item).ToString());
                                    continue;
                                    //build a final node
                                }
                                else if (item.FieldType.GetRuntimeFields() != null)
                                {
                                    //build a final multi node
                                    TreeNode subSubTreeNode = new TreeNode();
                                    var ClassFields =   item.FieldType.GetRuntimeFields() ;
                                    var c = item.FieldType.GetFields();
                                    var d = item.FieldType.GetProperties();
                                    var e = item.CustomAttributes;
                                    var f = item.FieldType.GetFields().ToList();
                                    foreach (FieldInfo fi in ClassFields)
                                    {
                                        subSubTreeNode.Nodes.Add(fi.Name);
                                    }
                                    subTreeNode.Nodes.Add(subSubTreeNode);
                                }
                            }
                        }
                    }
                    rootTreeNode.Nodes.Add(subTreeNode);
                }
            }
            else
            {
                if (hasFields(tmp.GetType()))
                {
                    var obFields = (tmp.GetType() as Type).GetRuntimeFields();

                    TreeNode subTreeNode = new TreeNode(tmp.GetType().Name);
                    foreach (FieldInfo item in obFields)
                    {

                        if (item.FieldType.Name.Contains("Enum"))
                        {
                            subTreeNode.Nodes.Add($"{item} as enum");
                            continue;
                            //build a final node
                        }
                        if (item.FieldType.Name == "String")
                        {
                            subTreeNode.Nodes.Add(item.GetValue(item).ToString());
                            continue;
                            //build a final node
                        }
                        else if (CheckIfValueType(item.FieldType))
                        {
                            subTreeNode.Nodes.Add(item.GetValue(item).ToString());
                            continue;
                            //build a final node
                        }
                        else if (item.FieldType.GetRuntimeFields() != null)
                        {
                            //build a final multi node


                            var ClassFields = (item.FieldType as Type).GetRuntimeFields();
                            foreach (FieldInfo fi in ClassFields)
                            {
                                TreeNode subSubTreeNode = new TreeNode();
                                if (fi.FieldType.FullName is string)
                                {
                                    subSubTreeNode.Nodes.Add(fi.Name);
                                    continue;
                                    //build a final node
                                    //add to multi node
                                }
                                else if (CheckIfValueType(fi.FieldType))
                                {
                                    subSubTreeNode.Nodes.Add(fi.Name);
                                    continue;
                                    //build a final node
                                    //add to multi node
                                }
                                else
                                {
                                    //TO DO => further
                                    subSubTreeNode.Nodes.Add(fi.Name);
                                }
                                subTreeNode.Nodes.Add(subSubTreeNode);
                            }

                        }

                    }
                }
            }
            return rootTreeNode;
        }

       
    }
    public class Container
    {
        public String Text { get; set; } = " ";
    }
}
