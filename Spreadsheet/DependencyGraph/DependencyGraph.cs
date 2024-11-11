// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024
//     Author: Minh Quoc Vo & Ryan Boardman
//     Date: 9/13/2024

namespace CS3500.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// This private int variable keeps track of the pairs of node inside the graph
    /// </summary>
    private int size;

    /// <summary>
    /// This Dictionary links node name to its set of dependent nodes  
    /// </summary>
    private Dictionary<string, HashSet<String>> dependents;

    /// <summary>
    /// This Dictionary links node name to its set of dependee nodes  
    /// </summary>
    private Dictionary<string, HashSet<String>> dependees;

    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        size = 0;
        this.dependents = new Dictionary<String, HashSet<String>>();
        this.dependees = new Dictionary<String, HashSet<String>>();
    }

    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return this.size; }
    }

    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        return this.dependents.ContainsKey(nodeName);
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        return this.dependees.ContainsKey(nodeName);
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        HashSet<String>? setDependents = new HashSet<String>();
        if(dependents.TryGetValue(nodeName, out setDependents)){
            return setDependents;
        }
        return new HashSet<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        HashSet<String>? setDependees = new HashSet<String>();
        if (dependees.TryGetValue(nodeName, out setDependees))
        {
            return setDependees;
        }
        return new HashSet<string>();
    }

    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        if (!this.dependents.ContainsKey(dependee))
        {
            this.dependents.Add(dependee, new HashSet<String>()); 
        }
        else if (this.dependents[dependee].Contains(dependent)){ // Check for duplications
            return;
        }
        this.dependents[dependee].Add(dependent);

        if (!this.dependees.ContainsKey(dependent))
        {
            this.dependees.Add(dependent, new HashSet<String>());
        }
        this.dependees[dependent].Add(dependee);

        this.size++;
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists.
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        if (this.dependents.ContainsKey(dependee))
        {
            this.dependents[dependee].Remove(dependent);
            if(this.dependents[dependee].Count == 0)
            {
                this.dependents.Remove(dependee);
            }
            this.dependees[dependent].Remove(dependee);
            if (this.dependees[dependent].Count == 0)
            {
                this.dependees.Remove(dependent);
            }
            size--;
        }
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced </param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        if (this.dependents.ContainsKey(nodeName))
        {
            foreach(string oldDependent in this.dependents[nodeName])
            {
                this.RemoveDependency(nodeName, oldDependent);
            }
            foreach(string newDependent in newDependents)
            {
                this.AddDependency(nodeName, newDependent); 
            }
        } else
        {
            foreach (string newDependent in newDependents)
            {
                this.AddDependency(nodeName, newDependent);
            }
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        if (this.dependees.ContainsKey(nodeName))
        {
            foreach (string oldDependee in this.dependees[nodeName])
            {
                this.RemoveDependency(oldDependee, nodeName);
            }
            foreach(string newDependee in newDependees)
            {
                this.AddDependency(newDependee,nodeName);
            }
        } else
        {
            foreach (string newDependee in newDependees)
            {
                this.AddDependency(newDependee, nodeName);
            }
        }
    }
}