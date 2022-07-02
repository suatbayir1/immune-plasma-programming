using System;
using System.Collections.Generic;
using System.Text;

namespace ffp
{
    class Node : ICloneable
    {
        // class static variables
        public static int id = 0;

        // instance attributes
        public string value;
        public int parent_id;
        public string type;
        public int node_id;
        public int depth;

        public Node(int parent_id, string value = "", string type = "")
        {
            this.parent_id = parent_id;
            this.node_id = id;
            this.type = type;
            id += 1;
        }

        public void Set_value(string[] source, string type)
        {
            Random rnd = new Random();
            this.value = source[rnd.Next(0, source.Length)];
            this.type = type;
        }

		public object Clone()
        {
            Node new_node = new Node(this.parent_id);
            new_node.value = this.value;
            new_node.type = this.type;
			new_node.node_id = this.node_id;
            return new_node;
        }
    }
}