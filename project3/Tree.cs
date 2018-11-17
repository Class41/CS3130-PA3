using System;
using System.Collections.Generic;

namespace project3
{
    class Tree
    {
        private static readonly List<int> values = new List<int> { 30, 10, 45, 38, 20, 50, 25, 33, 8, 12 }; //arraylist of provided values
        public static Node ROOT = null;

        static void Main(string[] args) => MenuControl(); //delegate main function to MenuController

        public static void MenuControl()
        {
            InitTree();
            while (true)
            {
                DrawMenu();
                char choice = Console.ReadLine()[0];
                if (choice == 'x')
                    Environment.Exit(0);

                switch (choice)
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
                                Console.WriteLine("Item not found in BST.");
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

        private static void InitTree()
        {
            foreach (var val in values)
            {
                TreeInsert(val);
            }
        }

        public static void DrawMenu()
        {
            Console.WriteLine(
                "\nChoose an option:" +
                "\n1 - Traverse Tree" +
                "\n2 - Insert Node" +
                "\n3 - Search for Node" +
                "\n4 - Delete Node" +
                "\nx - Exit"
                );
        }

        private static void TreeDeleteNode(int val)
        {
            var result = TreeSearch(val);

            if (result == null)
            {
                Console.WriteLine("\nFailed to delete node. Node does not exist in tree.");
                return;
            }

            if (result.self._nodeLeft == null && result.self._nodeRight == null)
            {
                if (result.left)
                    result.parent._nodeLeft = null;
                else
                    result.parent._nodeRight = null;

                return;
            }

            if (result.self._nodeLeft == null && result.self._nodeRight != null)
            {
                if (result.left)
                    result.parent._nodeLeft = result.self._nodeRight;
                else
                    result.parent._nodeRight = result.self._nodeRight;

                return;
            }

            if (result.self._nodeRight == null && result.self._nodeLeft != null)
            {
                if (result.left)
                    result.parent._nodeLeft = result.self._nodeLeft;
                else
                    result.parent._nodeRight = result.self._nodeLeft;

                return;
            }

            if (result.self._nodeRight != null && result.self._nodeLeft != null)
            {
                NodeSearchDetail successor = GetSuccessor(result.self);

                if (successor.parent == null)
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
                    if (successor.left)
                        successor.parent._nodeLeft = successor.self._nodeRight;

                    successor.self._nodeLeft = result.self._nodeLeft;
                    successor.self._nodeRight = result.self._nodeRight;

                    if (result.parent != null)
                    {
                        if (result.left)
                            result.parent._nodeLeft = successor.self;
                        else
                            result.parent._nodeRight = successor.self;
                    }
                    else
                    {
                        ROOT = successor.self;
                    }

                }
            }
        }

        private static NodeSearchDetail GetSuccessor(Node start)
        {
            Node parent, current = start._nodeRight;

            if (current._nodeLeft == null && current._nodeRight == null)
                return new NodeSearchDetail { self = current, parent = null };

            do
            {
                parent = current;
                current = current._nodeLeft;
            } while (current._nodeLeft != null);


            return new NodeSearchDetail { self = current, parent = parent, left = (parent._nodeLeft == current) ? true : false };
        }

        private static NodeSearchDetail TreeSearch(int val)
        {
            Node current = ROOT, parent;

            if (current == null)
                return null;
            if (current._value == val)
                return new NodeSearchDetail() { parent = null, self = current, left = false };

            do
            {
                if (current._value < val)
                {
                    parent = current;
                    current = current._nodeRight;
                }
                else
                {
                    parent = current;
                    current = current._nodeLeft;
                }
            } while (current != null && current._value != val);

            if (current != null)
                return new NodeSearchDetail() { parent = parent, self = current, left = (parent._nodeLeft == current) ? true : false };

            return null;
        }

        private static void TreeInsert(int val)
        {
            if (ROOT == null)
            {
                ROOT = new Node(val);
                return;
            }

            Node current, parent;
            bool left;
            current = ROOT;

            do
            {
                if (val > current._value && current != null)
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

        private static void TreeTraverse()
        {
            Console.WriteLine("\nIn-Order: ");
            TraverseInOrder(ROOT);
            Console.WriteLine("\nPre-Order: ");
            TraversePreOrder(ROOT);
            Console.WriteLine("\nPost-Order: ");
            TraversePostOrder(ROOT);
        }

        private static void TraversePostOrder(Node start)
        {
            if (start != null)
            {
                TraversePostOrder(start._nodeLeft);
                TraversePostOrder(start._nodeRight);
                Console.Write(start._value + " . ");
            }
        }

        private static void TraversePreOrder(Node start)
        {
            if (start != null)
            {
                Console.Write(start._value + " . ");
                TraversePreOrder(start._nodeLeft);
                TraversePreOrder(start._nodeRight);
            }
        }

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

    public class NodeSearchDetail
    {
        public Node parent, self;
        public bool left;
    }

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
