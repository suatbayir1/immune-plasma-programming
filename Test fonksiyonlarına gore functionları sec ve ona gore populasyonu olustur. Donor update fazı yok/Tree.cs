using System;
using System.Collections.Generic;
using System.Text;

namespace ffp
{
    class Tree : ICloneable
    {
        // class static variables
        public static string[] operators = { "+", "-", "*", "/" };
       // public static string[] functions = { "sin", "cos", "exp", "rlog" };
        public static string[] functions = { "same" };
        public static string[] terminals = { "x", "1" };
        public static double operator_rate = 0.5;
        public static double terminal_rate = 0.5;
        public static double sharing_rate  = 0.9;

        // instance attributes
        public Node root;
        public Tree left = null;
        public Tree right = null;
        public double error = 10e6;
        Random random = new Random();

        public Tree(int parent_id = -1)
        {
            this.root = new Node(parent_id: parent_id);
        }

        public void Create_tree(string method, int min_depth, int max_depth, int current_depth = 0)
        {
            if (method == "full")
            {
                if (current_depth < max_depth - 1)
                {
                    if (random.NextDouble() > operator_rate)
                    {
                        this.root.Set_value(functions, "function");
                    }
                    else
                    {
                        this.root.Set_value(operators, "operator");
                    }
                }
                else
                {
                    this.root.Set_value(terminals, "terminal");
                }
            }
            else if (method == "grow")
            {
                if (current_depth < min_depth)
                {
                    if (random.NextDouble() > operator_rate)
                    {
                        this.root.Set_value(functions, "function");
                    }
                    else
                    {
                        this.root.Set_value(operators, "operator");
                    }
                }
                else if (current_depth >= min_depth && current_depth < max_depth - 1)
                {
                    if (random.NextDouble() > terminal_rate)
                    {
                        if (random.NextDouble() > operator_rate)
                        {
                            this.root.Set_value(functions, "function");
                        }
                        else
                        {
                            this.root.Set_value(operators, "operator");
                        }
                    }
                    else
                    {
                        this.root.Set_value(terminals, "terminal");
                    }
                }
                else
                {
                    this.root.Set_value(terminals, "terminal");
                }
            }
            
            if (this.root.type == "operator")
            {
                this.left = new Tree(parent_id: this.root.node_id);
                this.left.Create_tree(method, min_depth, max_depth, current_depth + 1);
            }
            if (this.root.type == "operator" || this.root.type == "function")
            {
                this.right = new Tree(parent_id: this.root.node_id);
                this.right.Create_tree(method, min_depth, max_depth, current_depth + 1);
            }
        } // end of Create_tree method

        public string Tree_equation()
        {
            string equation = "(";
            if (this.left != null)
            {
                equation += this.left.Tree_equation();
            }
            equation += this.root.value;
            if (this.right != null)
            {
                equation += this.right.Tree_equation();
            }
            equation += ")";
            return equation;
        } // end of Tree_equation method

        public List<Node> Get_nodes()
        {
            var nodes = new List<Node>();
            nodes.Add(this.root);
            if (this.left != null)
            {
                nodes.AddRange(this.left.Get_nodes());
            }
            if (this.right != null)
            {
                nodes.AddRange(this.right.Get_nodes());
            }
            return nodes;
        } // end of Get_nodes method

        // public int Tree_depth()
        // {
        //     int depth_left = 0;
        //     int depth_right = 0;

        //     if (this.left != null)
        //     {
        //         depth_left = this.left.Tree_depth();
        //     }

        //     if (this.right != null)
        //     {
        //         depth_right = this.right.Tree_depth();
        //     }

        //     return Math.Max(depth_left, depth_right) + 1;
        // } // end of Tree_depth method

        public int Tree_depth()
        {
            int depth_left = 0;
            int depth_right = 0;

            if (this.left != null)
            {
                depth_left = this.left.Tree_depth();
            }

            if (this.right != null)
            {
                depth_right = this.right.Tree_depth();
            }

            return Math.Max(depth_left, depth_right) + 1;
        } // end of Tree_depth method

        public void Control_depth()
        {
            if (this.root.depth > Par.max_depth)
            {
                this.root.type = "terminal";
                this.root.value = Par.max_depth_value;
                this.left = null;
                this.right = null;
            }
            else {
                if (this.left != null)
                    this.left.Control_depth();
                if (this.left != null)
                    this.left.Control_depth();
            }
        }


        public void Assign_depth(int depth = 1)
        {
            this.root.depth = depth;
            if (this.left != null)
                this.left.Assign_depth(depth + 1);
            if (this.right != null)
                this.right.Assign_depth(depth + 1);
        }

        public object Clone()
        {
            Tree new_tree = new Tree();
            new_tree.root = (Node)this.root.Clone();
            new_tree.error = this.error; 
            if (this.left != null)
                new_tree.left = (Tree)this.left.Clone();
            else
                new_tree.left = null;
            if (this.right != null)
                new_tree.right = (Tree)this.right.Clone();
            else
                new_tree.right = null;
            return new_tree;
        } // end of Clone method 


        public Node Random_node(string type = "")
        {
            List<Node> nodes = this.Get_nodes();
            Node selected_node = null;
            Boolean correct_selected = false;

            if (type == "function")
            {
                Boolean has_non_terminals = false;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].type != "terminal")
                    {
                        has_non_terminals = true;
                        break;
                    }
                }

                if (has_non_terminals)
                {
                    while (!correct_selected)
                    {
                        selected_node = nodes[random.Next(0, nodes.Count)];
                        if (selected_node.type != "terminal")
                            correct_selected = true;
                    }
                }
                else 
                {
                    selected_node = nodes[random.Next(0, nodes.Count)];
                }
            }
            else if  (type == "terminal")
            {
                while (!correct_selected)
                {
                    selected_node = nodes[random.Next(0, nodes.Count)];
                    if (selected_node.type == "terminal")
                        correct_selected = true;
                }
            }
            else
            {
                selected_node = nodes[random.Next(0, nodes.Count)];
            }
            return selected_node;
        } // end of Random_node method

        public object Copy_subtree()
        {
            Node selected_node = null;

            if (random.NextDouble() < sharing_rate)
                selected_node = this.Random_node("function");
            else
                selected_node = this.Random_node("terminal");

            int share_point = selected_node.node_id;
            return this.Find_tree(share_point, "copy");
        } // end of Copy_subtree method 

        public void Paste_subtree(Tree subtree)
        {
            Node selected_node = null;
            if (subtree.root.type != "terminal")
                selected_node = this.Random_node("function");
            else
                selected_node = this.Random_node("terminal");
            int share_point = selected_node.node_id;
            this.Find_tree(share_point, "paste", subtree);
        } // end of Paste_subtree method

        public void Glue_instance(Tree subtree)
        {
            int parent_id = this.root.parent_id;
            this.root = (Node)subtree.root.Clone();
            this.root.parent_id = parent_id;
            this.error = subtree.error;

            if (subtree.left != null)
                this.left = (Tree)subtree.left.Clone();
            else
                this.left = null;

            if (subtree.right != null)
                this.right = (Tree)subtree.right.Clone();
            else 
                this.right = null;
        } // end of Glue_instance method 

        public void Change_subtree()
        {
            Random random = new Random();
            int rand = random.Next(0, 3);
            string[] TARGET;
            string TYPE;
            if (rand == 1)
            {
                TARGET = operators;
                TYPE = "operator";
            }
            else if (rand == 2)
            {
                TARGET = functions;
                TYPE = "function";
            }
            else 
            {
                TARGET = terminals;
                TYPE = "terminal";
            }
            int rand2 = random.Next(0, 2);
            string method;
            if (rand2 == 0)
                method = "grow";
            else
                method = "full";
            
            string type_before = this.root.type;
            this.root.Set_value(TARGET, TYPE);
            
            if (type_before == "operator") 
            {
                if (rand == 2 || rand == 0)
                    this.left = null;
                if (rand == 0)
                    this.right = null;
            }
            else if (type_before == "function")
            {
                if (rand == 1)
                {
                    this.left = new Tree(parent_id: this.root.node_id);
                    this.left.Create_tree(method, Par.init_min_depth, Par.init_max_depth);
                }
                if (rand == 0)
                {
                    this.left = null;
                    this.right = null;
                }
            }
            else {
                if (rand == 1)
                {
                    this.left = new Tree(parent_id: this.root.node_id);
                    this.left.Create_tree(method, Par.init_min_depth, Par.init_max_depth);
                }
                if (rand == 1 || rand == 2)
                {
                    this.right = new Tree(parent_id: this.root.node_id);
                    this.right.Create_tree(method, Par.init_min_depth, Par.init_max_depth);
                }
            }
        }

        public void Change_node()
        {
            Node selected_node = this.Random_node();
            this.Find_tree(selected_node.node_id, "change");
            this.Assign_depth();
        }

        public object Find_tree(int root_id, string process, Tree subtree = null)
        {
            Tree tree = null;
            if (root_id == this.root.node_id)
            {
                if (process == "copy")
                {
                    tree = (Tree)this.Clone();
                }
                else if (process == "paste") 
                {
                    this.Glue_instance(subtree);
                    return null;
                }
                else if (process == "change") 
                {
                    this.Change_subtree();
                    return null;
                }
            }
            else
            {
                List<Node> nodes_left;
                int in_left = -1;
            
                if (this.left != null)
                {
                    nodes_left = this.left.Get_nodes();
                    int[] nodes_id = new int[nodes_left.Count];
                    for (int i = 0; i < nodes_left.Count; i++)
                        nodes_id[i] = nodes_left[i].node_id;
                    in_left = Array.IndexOf(nodes_id, root_id);
                }

                if (this.left != null && in_left > -1)
                {
                    tree = (Tree)this.left.Find_tree(root_id, process, subtree);
                }
                else
                {
                    tree = (Tree)this.right.Find_tree(root_id, process, subtree);
                }  
            }
            return tree;
        } // end of Find_tree method 

        public double Calc_tree(double x, double y = 0)
        {
            double result = 1e30;
            
            if (this.root.type == "operator")
            {
                if (this.root.value == "+")
                    result = this.left.Calc_tree(x, y) + this.right.Calc_tree(x, y);
                else if (this.root.value == "-")
                    result = this.left.Calc_tree(x, y) - this.right.Calc_tree(x, y);
                else if (this.root.value == "*")
                    result = this.left.Calc_tree(x, y) * this.right.Calc_tree(x, y);
                else if (this.root.value == "/")
                {
                    double right_result = this.right.Calc_tree(x, y);
                    if (right_result == 0)
                    {
                        result = 1;
                        this.root.value = "1";
                        this.root.type = "terminal";
                        this.left = null;
                        this.right = null;
                    }
                    else
                        result = this.left.Calc_tree(x, y) / right_result;
                }
            }
            else if (this.root.type == "function")
            {
                if (this.root.value == "sin")
                    result = Math.Sin(this.right.Calc_tree(x, y));
                else if (this.root.value == "cos")
                    result = Math.Cos(this.right.Calc_tree(x, y));
                else if (this.root.value == "exp")
                    result = Math.Exp(this.right.Calc_tree(x, y));
                else if (this.root.value == "rlog")
                {
                    double right_result = this.right.Calc_tree(this.right.Calc_tree(x, y));
                    if (right_result == 0)
                    {
                        result = 0;
                        this.root.value = "0";
                        this.root.type = "terminal";
                        this.right = null;
                    }
                    else
                        result = Math.Log(Math.Abs(right_result));
                } else if (this.root.value == "same")
                {
                    result = this.right.Calc_tree(x, y);
                }
            }
            else
            {
                if (this.root.value == "x")
                    result = x;
                else if (this.root.value == "y")
                    result = y;
                else
                    result = Double.Parse(this.root.value);
            }
            return result;
        } // end of Calc_tree method


        public void Fix_tree()
        {
            if (this.root.type == "terminal")
            {
                this.left = null;
                this.right = null;
            }
            else
            {
                if (this.left != null)
                    this.left.Fix_tree();
                if (this.right != null)
                    this.right.Fix_tree();
            }
        } // end of Fix_tree method

        public void Simplify_tree()
        {
            if (Par.Simplify)
            {
                if (this.left != null)
                    this.left.Simplify_tree();
                if (this.right != null)
                    this.right.Simplify_tree();
                if (this.root.type == "operator")
                {
                    double result = this.Simplify();
                    if (result != 1e30)
                    {
                        this.root.type = "terminal";
                        this.root.value = result.ToString();
                        this.left = null;
                        this.right = null;
                    }
                }
            }

        }

        public double Simplify()
        {
            double result = 0;
            
            if (this.root.type == "operator")
            {
                try
                {
                    if (this.root.value == "+")
                        result = Double.Parse(this.left.root.value) + Double.Parse(this.right.root.value);
                    else if (this.root.value == "-")
                        result = Double.Parse(this.left.root.value) - Double.Parse(this.right.root.value);
                    else if (this.root.value == "*")
                        result = Double.Parse(this.left.root.value) * Double.Parse(this.right.root.value);
                    else if (this.root.value == "/")
                        result = Double.Parse(this.left.root.value) / Double.Parse(this.right.root.value);
                }
                catch
                {
                    result = 1e30;
                }
            }
            return result;
        }


        public void Update_error()
        {
            double[] points_x = Line_space(Par.domain_x[0], Par.domain_x[1], Par.point_num);
			double[] points_y = Line_space(Par.domain_y[0], Par.domain_y[1], Par.point_num);
            double targetValue = 0;
            double obtainedValue = 0;
            double totalError = 0;
            for (int i = 0; i < Par.point_num; i++)
            {
                targetValue = Par.Target_function(points_x[i], points_y[i]);
                obtainedValue = this.Calc_tree(points_x[i], points_y[i]);
                totalError += Math.Abs(targetValue - obtainedValue);
            }

            this.error = totalError;
        } // end of Update_error method

        public static double[] Line_space(double x1, double x2, int num)
		{
            Random random = new Random();
			double[] line = new double[num];
			double step_size = Math.Abs(x2 - x1) / num;
            
            int rem_num = num;
			for (int i = 0; i < num; i++)
            {
                double upperB = x1 + ((i + 1) * step_size);
                double lowerB = upperB - step_size;
				line[i] = random.NextDouble() * (upperB - lowerB) + lowerB;
			}
			return line;
        } // end of Line_space method 
    }
}
