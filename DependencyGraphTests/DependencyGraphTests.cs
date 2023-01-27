using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      26 Jan 2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Matthew Goh - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Matthew Goh, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    /// 
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should have size 0.
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        /// Adding to DG should alter size as well as contain specified dependees and dependents
        ///</summary>
        [TestMethod()]
        public void SimpleAddTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);

            // Adding the same pair should not increase size...
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);

            // Ensure that we have the dependents and dependees in the dictionaries
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("a"));

            t.AddDependency("b", "a");
            t.AddDependency("t", "b");
            Assert.AreEqual(3, t.Size);
            Assert.IsTrue(t.HasDependees("t"));
            Assert.IsTrue(t.HasDependees("b"));
            Assert.AreEqual(2, t["b"]);
        }

        /// <summary>
        /// Test if the right dependee size is returned.
        /// </summary>
        [TestMethod()]
        public void TestDependeeSize()
        {
            DependencyGraph t = new DependencyGraph();

            // 0 should be returned if "a" is not a dependent
            Assert.AreEqual(0, t["a"]);

            // Dependee size should be returned appropriately
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t["b"]);
            t.AddDependency("a", "b");
            t.AddDependency("g", "b");
            t.AddDependency("h", "b");
            Assert.AreEqual(3, t["b"]);
            Assert.AreEqual(0, t["g"]);
        }

        /// <summary>
        /// Test if existence of certain dependents are correctly returned.
        /// </summary>
        [TestMethod()]
        public void TestHasDependents()
        {
            DependencyGraph t = new DependencyGraph();

            // Return false if the specified dependents do not exist
            Assert.IsFalse(t.HasDependents("g"));
            t.AddDependency("a", "b");
            t.AddDependency("c", "d");
            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependents("c"));
            Assert.IsFalse(t.HasDependents("e"));

            // Return true if it does exist
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependents("d"));
        }

        /// <summary>
        /// Test if existence of certain dependees are correctly returned.
        /// </summary>
        [TestMethod()]
        public void TestHasDependees()
        {
            DependencyGraph t = new DependencyGraph();

            // Return false if the specified dependents do not exist
            Assert.IsFalse(t.HasDependees("g"));
            t.AddDependency("a", "b");
            t.AddDependency("c", "d");
            Assert.IsFalse(t.HasDependees("b"));
            Assert.IsFalse(t.HasDependees("d"));
            Assert.IsFalse(t.HasDependees("e"));

            // Return true if it does exist
            Assert.IsTrue(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependees("c"));
        }

        /// <summary>
        /// Tests the remove method
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
            t.AddDependency("a", "b");

            // Removing these ordered pairs should not affect the contents and size.
            t.RemoveDependency("x", "b");
            t.RemoveDependency("a", "x");
            Assert.AreEqual(1, t.Size);
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependees("a"));

            // Ensure that the right size is returned for removed dependencies.
            t.AddDependency("b", "a");
            t.AddDependency("d", "b");
            Assert.AreEqual(3, t.Size);
            t.RemoveDependency("d", "b");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("b", "a");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        /// Test an empty Enumerator.
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possible to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        /// Correct size should be returned, removing unknown pairs should not alter size
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
            t.RemoveDependency("a", "b");
            Assert.AreEqual(3, t.Size);
            t.RemoveDependency("a", "g");
            Assert.AreEqual(3, t.Size);
        }


        /// <summary>
        /// Test non-empty enumerator.
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        /// Test combination of replace and enumerate.
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}
