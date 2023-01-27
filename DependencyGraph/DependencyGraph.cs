// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SpreadsheetUtilities
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
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // Two dictionary fields and int varuable to store dependents, dependees, and the amount of ordered pairs.
        Dictionary<String, HashSet<String>> dependents;
        Dictionary<String, HashSet<String>> dependees;
        int numOfPairs = 0;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<String, HashSet<String>>();
            dependees = new Dictionary<String, HashSet<String>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return numOfPairs; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get 
            {
                // If dependents have the input value, return all the dependees associated with it.
                if (dependents.ContainsKey(s))
                {
                    return dependents[s].Count;
                }
                else
                    return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s)) 
            { 
                return true; 
            }
            else 
                return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependees.ContainsKey(s))
            {
                // Return a copied set of all dependents at s.
                HashSet<string> dependentsAts = new HashSet<string>(dependees[s]);
                return dependentsAts;
            }
            else 
                return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (dependents.ContainsKey(s))
            {
                // Return a copied set of all dependees at s.
                HashSet<string> dependeesAtS = new HashSet<string>(dependents[s]);
                return dependeesAtS;
            }
            else
                return Enumerable.Empty<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///   t = dependent
        ///   s = dependee
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // If dependent or dependee does not exist in dictionary, add count
            if (!dependees.ContainsKey(s) || !dependents.ContainsKey(t))
            {
                numOfPairs++;
            }

            // If dependees does not contain the key, add dependee and the set of dependents to dictionary
            if (!dependees.ContainsKey(s))
            {
                HashSet<string> dependentsSet = new HashSet<string>();
                dependentsSet.Add(t);
                dependees.Add(s, dependentsSet);
            }
            // If it does contain the key, add the dependents to "s."
            else
            {
                dependees[s].Add(t);
            }

            // If dependents does not contain the key, add dependent and the set of dependees
            if (!dependents.ContainsKey(t))
            {
                HashSet<string> dependeesSet = new HashSet<string>();
                dependeesSet.Add(s);
                dependents.Add(t, dependeesSet);
            }
            // If it does contain the key, add dependee to "t."
            else
            {
                dependents[t].Add(s);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // If dependent or dependee does exist in dictionaries, remove dependent and
            // dependee from dictionaries and decrease count
            if (dependees.ContainsKey(s) && dependents.ContainsKey(t))
            {
                dependees[s].Remove(t);
                dependents[t].Remove(s);
                numOfPairs--;
            }

            // If dependee contains key and there are no more dependents, remove the key.
            if (dependees.ContainsKey(s) && dependees[s].Count == 0)
            {
                dependees.Remove(s);
            }

            // If dependent contains key and has no more dependees, remove the key.
            if (dependents.ContainsKey(t) && dependents[t].Count == 0)
            {
                dependents.Remove(t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // Set dependents of s to a new Enumerable.
            IEnumerable <string> removeDependents = GetDependents(s);
            foreach (string dependent in removeDependents)
            {
                RemoveDependency(s, dependent);
            }

            foreach(string dependent in newDependents)
            {
                AddDependency(s, dependent);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // Set dependees of s to a new Enumerable.
            IEnumerable<string> removeDependees = GetDependees(s);
            foreach (string dependee in removeDependees)
            {
                RemoveDependency(dependee, s);
            }

            foreach (string dependee in newDependees)
            {
                AddDependency(dependee, s);
            }
        }

    }

}
