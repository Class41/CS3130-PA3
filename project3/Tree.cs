using System;
using System.Collections.Generic;

namespace project3
{
    class Tree
    {
        private static readonly List<int> values = new List<int> { 30, 10, 45, 38, 20, 50, 25, 33, 8, 12 }; //arraylist of provided values
        public static Node ROOT = null;

        static void Main(string[] args) => MenuControl(); //delegate main function to MenuController

        /// <summary>
        /// <para>Menu loop and initial tree call</para>
        /// </summary>
        public static void MenuControl()
        {
            InitTree();
            while (true)
            {
                DrawMenu();
                char choice = Console.ReadLine()[0]; // Get character from the stdin
                if (choice == 'x') // Short-circuit the loop by killing the program prematurely if x is selected.
                    Environment.Exit(0);

                switch (choice) // Menu component - choose what type of action you wish to perform
                {
                    case '1':
                        TreeTraverse();
                        break;
                    case '2':
                        Console.WriteLine("\nWhat value to insert?");
                        try
                        {
                            TreeInsert(int.Parse(Console.ReadLine()));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\nBad Input");
                        }
                        break;
                    case '3':
                        Console.WriteLine("\nWhat value to search for?");
                        try
                        {
                            var result = TreeSearch(int.Parse(Console.ReadLine()));
                            if (result != null)
                            {
                                if (result.parent != null)
                                    Console.WriteLine(string.Format("\nFound value: {0}, with parent {1}", result.self._value, result.parent._value));
                                else
                                    Console.WriteLine(string.Format("\nFound value: {0}, at root.", result.self._value));
                            }
                            else
                            {
                                Console.WriteLine("\nItem not found in BST.");
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\nBad Input");
                        }
                        break;
                    case '4':
                        Console.WriteLine("\nWhat value to delete?");
                        try
                        {
                            TreeDeleteNode(int.Parse(Console.ReadLine()));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("\nBad Input");
                        }
                        break;
                    default:
                        Console.WriteLine("\nInvalid input.");
                        break;
                }
            }
        }

        /// <summary>
        /// <para>Populates the initial tree using the tree insert method</para>
        /// </summary>
        private static void InitTree()
        {
            foreach (var val in values)
            {
                TreeInsert(val);
            }
        }

        /// <summary>
        /// <para>Draws the menu</para>
        /// </summary>
        public static void DrawMenu()
        {
            Console.WriteLine(
                "\n\nChoose an option:" +
                "\n1 - Traverse Tree" +
                "\n2 - Insert Node" +
                "\n3 - Search for Node" +
                "\n4 - Delete Node" +
                "\nx - Exit"
                );
        }

        /// <summary>
        /// <para>Handles node del</para>
        /// </summary>
        /// <param name="val"></param>
        private static void TreeDeleteNode(int val)
        {
            var result = TreeSearch(val); //Search the tree to see if the node exists

            if (result == null) //node does not exist
            {
                Console.WriteLine("\nFailed to delete node. Node does not exist in tree.");
                return;
            }

            //Case1: no children : simply wipe the node from the parent
            if (result.self._nodeLeft == null && result.self._nodeRight == null) 
            {
                if (result.left)
                    result.parent._nodeLeft = null;
                else
                    result.parent._nodeRight = null;

                return;
            }

            //Case2: has a right child but no left child : remove current node set child as child of parent
            if (result.self._nodeLeft == null && result.self._nodeRight != null) 
            {
                if (result.left)
                    result.parent._nodeLeft = result.self._nodeRight;
                else
                    result.parent._nodeRight = result.self._nodeRight;

                return;
            }
            //Case3: Has a left child but not right child : remove current node set child as child of parent
            if (result.self._nodeRight == null && result.self._nodeLeft != null) 
            {
                if (result.left)
                    result.parent._nodeLeft = result.self._nodeLeft;
                else
                    result.parent._nodeRight = result.self._nodeLeft;

                return;
            }
            //Case4: Has both children : Get successor, take over the children of deletion node, adjust the children of successor
            if (result.self._nodeRight != null && result.self._nodeLeft != null) 
            {
                NodeSearchDetail successor = GetSuccessor(result.self);

                if (successor.parent == null) //if successor is the immediate right node of the deletion node
                {
                    if (result.left)
                    {
                        result.parent._nodeLeft = successor.self;
                        successor.self._nodeLeft = result.self._nodeLeft;
                    }
                    else
                    {
                        result.parent._nodeRight = successor.self;
                        successor.self._nodeLeft = result.self._nodeLeft;
                    }
                }
                else
                {
                    if (successor.left) //since successor is always left-most child of right sub tree, successor can't have left child. 
                        successor.parent._nodeLeft = successor.self._nodeRight;

                    successor.self._nodeLeft = result.self._nodeLeft; //take over the duties of deletion node
                    successor.self._nodeRight = result.self._nodeRight;

                    if (result.parent != null) //update parent to point to the successor now
                    {
                        if (result.left)
                            result.parent._nodeLeft = successor.self;
                        else
                            result.parent._nodeRight = successor.self;
                    }
                    else
                    {
                        ROOT = successor.self; //if the root is being deleted
                    }

                }
            }
        }

        /// <summary>
        /// <para>Returns the successor along with its parent</para>
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private static NodeSearchDetail GetSuccessor(Node start)
        {
            Node parent, current = start._nodeRight;

            if (current._nodeLeft == null && current._nodeRight == null) //if it turns out that this is a leaf
                return new NodeSearchDetail { self = current, parent = null };

            do
            {
                parent = current;
                current = current._nodeLeft;
            } while (current._nodeLeft != null); //keep going left until we can't anymore


            return new NodeSearchDetail { self = current, parent = parent, left = (parent._nodeLeft == current) ? true : false };
        }

        /// <summary>
        /// <para>Searches tree for a specific value</para>
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static NodeSearchDetail TreeSearch(int val)
        {
            Console.Write("\nExamining: ");
            Node current = ROOT, parent;

            if (current == null) //empty tree
                return null;
            if (current._value == val) //if the value is the root
                return new NodeSearchDetail() { parent = null, self = current, left = false };

            do
            {
                if (current != null)
                    Console.Write(current._value + " ");

                if (current._value < val) //compare if the value we are searching for is > or < than the current value node
                {
                    parent = current;
                    current = current._nodeRight;
                }
                else
                {
                    parent = current;
                    current = current._nodeLeft;
                }
            } while (current != null && current._value != val); //keep going until the current node is null and the value is not what we want

            if (current != null)
            {
                Console.Write(current._value + " ");
                return new NodeSearchDetail() { parent = parent, self = current, left = (parent._nodeLeft == current) ? true : false };
            }

            return null;
        }

        /// <summary>
        /// <para>Insert a value into the tree</para>
        /// </summary>
        /// <param name="val"></param>
        private static void TreeInsert(int val)
        {
            if (ROOT == null) //insert as root if root is null
            {
                ROOT = new Node(val);
                return;
            }

            Node current, parent;
            bool left;
            current = ROOT;

            do
            {
                if (val > current._value && current != null) //check > or < than root and insert as leaf accordingly
                {
                    parent = current;
                    current = current._nodeRight;
                    left = false;
                }
                else
                {
                    parent = current;
                    current = current._nodeLeft;
                    left = true;
                }
            } while (current != null);


            if (left)
                parent._nodeLeft = new Node(val);
            else
                parent._nodeRight = new Node(val);
        }

        /// <summary>
        /// <para>Traverses tree in all 3 ways and displays output</para>
        /// </summary>
        private static void TreeTraverse()
        {
            Console.WriteLine("\nIn-Order: ");
            TraverseInOrder(ROOT);
            Console.WriteLine("\nPre-Order: ");
            TraversePreOrder(ROOT);
            Console.WriteLine("\nPost-Order: ");
            TraversePostOrder(ROOT);
        }

        /// <summary>
        /// <para>Recursive post order function</para>
        /// </summary>
        /// <param name="start"></param>
        private static void TraversePostOrder(Node start)
        {
            if (start != null)
            {
                TraversePostOrder(start._nodeLeft);
                TraversePostOrder(start._nodeRight);
                Console.Write(start._value + " . ");
            }
        }

        /// <summary>
        /// <para>Recursive pre order function</para>
        /// </summary>
        /// <param name="start"></param>
        private static void TraversePreOrder(Node start)
        {
            if (start != null)
            {
                Console.Write(start._value + " . ");
                TraversePreOrder(start._nodeLeft);
                TraversePreOrder(start._nodeRight);
            }
        }

        /// <summary>
        /// <para>Recursive in order function</para>
        /// </summary>
        /// <param name="start"></param>
        private static void TraverseInOrder(Node start)
        {
            if (start != null)
            {
                TraverseInOrder(start._nodeLeft);
                Console.Write(start._value + " . ");
                TraverseInOrder(start._nodeRight);
            }
        }
    }

    /// <summary>
    /// <para>Complex class to hold return data for node and its parent</para>
    /// </summary>
    public class NodeSearchDetail
    {
        public Node parent, self;
        public bool left; //is self a left child of parent?
    }

    /// <summary>
    /// <para>Definition of a node</para>
    /// </summary>
    public class Node
    {
        public int _value;
        public Node _nodeLeft, _nodeRight;

        public Node(int val, Node left = null, Node right = null)
        {
            _nodeLeft = left;
            _nodeRight = right;
            _value = val;
        }
    }
}
