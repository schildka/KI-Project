using System;
using System.Collections.Generic;


namespace TreeSearch
{
     /// <summary>
    /// We require a simple tree data structure with a generic data attribute.
    /// A node has at most one parent, any number of children of the same type, a level in the tree(starting at 0 for the root).
    /// It has a traversal method that accepts a method of type `Action<Node<DataType>>`, wich is applied to the node, after wich `Traverse` is called recursively on the children.
    /// A node ensures it has at most one parent and it is the parent of all its children -- define the interface accordingly with the required methods and visibilities.
    /// </summary>
    public sealed class Node<DataType>
    {
        public bool isRoot;
        public DataType data;
        public String color;
        public Node<DataType> Vorgaenger = null;
        public int Value;
        public System.Collections.Generic.List<Node<DataType>> Nachfolger = new List<Node<DataType>>();
        public Node(DataType data)
        {
            this.data = data;
        }
        

        public void Traverse(Action<Node<DataType>> function)
        {
            if(this == null)return;
            if (Vorgaenger != null)
            {
                Console.WriteLine(data + "  child from  " + Vorgaenger.data);
            }
            else { Console.WriteLine(data + "  is Root" ); }
           
            function.Invoke(this);
            try { 
            foreach (Node<DataType> node in Nachfolger)
            {
                node.Vorgaenger = this;
                node.Traverse(function);
            }
            }catch(NullReferenceException e) { }

            ;
            // To be implemented
        }

        public void initBFS()
        {
            color = "white";
            foreach (Node<DataType> n in Nachfolger)
            {
                n.initBFS();
            }
        }
   
        

          // Complete the Node class...
    }

    

}

