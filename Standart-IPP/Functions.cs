using System;
using System.Collections.Generic;
using System.Text;

namespace ffp
{
    class Functions
    {
        public static Tree[] Init_population()
        {
            Tree[] pop = new Tree[Par.pop_size];
            
            for (int i = 0; i < Par.pop_size / 2; i++)
            {
                Tree tree = new Tree();
                tree.Create_tree("full", Par.init_min_depth, Par.init_max_depth);
                // tree.Simplify_tree();
                if (i > 0)
                {
                    Boolean different = Control_difference(pop, tree);
                    while (!different)
                    {
                        tree.Create_tree("full", Par.init_min_depth, Par.init_max_depth);
                        different = Control_difference(pop, tree);
                    }
                }
                tree.Update_error();
                pop[i] = tree;
            }
            for (int i = Par.pop_size / 2; i < Par.pop_size; i++)
            {
                Tree tree = new Tree();
                tree.Create_tree("grow", Par.init_min_depth, Par.init_max_depth);
                // tree.Simplify_tree();
                
                Boolean different = Control_difference(pop, tree);
                while (!different)
                {
                    tree.Create_tree("grow", Par.init_min_depth, Par.init_max_depth);
                    different = Control_difference(pop, tree);
                }
                tree.Update_error();
                pop[i] = tree;
            }
            return pop;
        } // end of Init_population method


        public static void Share(Tree t1, Tree t2)
        {
            Tree sub = (Tree)t1.Copy_subtree();
            t2.Paste_subtree(sub);
        } // end of share method


        public static Tree Check_depth(Tree tree)
        {
            string method;
            Random rnd = new Random();
            if (tree.Tree_depth() > Par.max_depth)
            {
                int rand = rnd.Next(0, 1);
                if (rand == 0)
                {
                    rand = rnd.Next(0, 1);
                    if (rand == 0)
                        method = "grow";
                    else
                        method = "full";
                    tree.Create_tree(method, Par.init_min_depth, Par.init_max_depth);
                    
                } 
                else
                {
                    tree.Assign_depth();
                    tree.Control_depth();
                }
            } else {
                // tree.Control_depth();
            }
            return tree;
        } // end of Check_depth method 
        
        
        public static Boolean Control_difference(Tree[] pop, Tree tree)
        {
            Boolean different = true;
            for (int i = 0; i < pop.Length; i++)
            {
                different = Is_different(pop[i], tree);
                if (!different)
                    break;
            }
            return different;
        } // end of Control_difference method 

        public static Boolean Is_different(Tree t1, Tree t2)
        {
            Boolean different = false;
            if (t1 != null && t2 != null)
            {
                different = Is_different(t1.left, t2.left);
                if (t1.root.value != t2.root.value)
                {
                    different = true;
                    return different;
                }
                if (!different)
                    different = Is_different(t1.right, t2.right);
            }
            else if ((t1 != null && t2 == null) || (t1 == null && t2 != null))
                different = true;
            return different;
        } // end of Is_different method 
    }
}