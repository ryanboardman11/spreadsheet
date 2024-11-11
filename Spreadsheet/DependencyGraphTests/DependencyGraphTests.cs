//     Author: Minh Quoc Vo & Ryan Boardman
//     Date: 9/13/2024
namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// This test stresses the DependencyGraph class by adding and removing a large amount of dependencies 
    /// </summary>
    [TestMethod]
    [Timeout(2000)]  // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();

        // Array of unique string ('a', 'b', ..., up to SIZE)
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // Empty set of dependents and dependees of size 200
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add dependencies of letters[i] depends on letters[j] (for j > i)
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        //Remove dependencies based on a step of 4
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // // re-add letters into the graph
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove dependencies based on a step of 3
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Check if the DependencyGraph works as expected
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    // --- Test For Constructor

    [TestMethod]
    public void DependencyGraphConstructor_TestCreateEmptyGraph_Valid()
    {
        _ = new DependencyGraph();
    }

    // --- Test For Size

    [TestMethod]
    public void DependencyGraphSize_TestSizeEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
        Assert.AreEqual(0, emptyGraph.Size);
    }

    [TestMethod]
    public void DependencyGraphSize_TestGraphSizeOne_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");
        Assert.AreEqual(1, graph.Size);
    }


    [TestMethod]
    public void DependencyGraphSize_TestGraphSizeThree_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");
        graph.AddDependency("A2", "B1");
        graph.AddDependency("A3", "B1");
        Assert.AreEqual(3, graph.Size);
    }

    // --- Test For HasDependents

    [TestMethod]
    public void DependencyGraphHasDependents_TestHasDependentsEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
       Assert.IsFalse(emptyGraph.HasDependents("A1")); 
    }

    [TestMethod]
    public void DependencyGraphHasDependents_TestHasOneDependent_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        Assert.IsTrue(graph.HasDependents("A1"));
    }

    /// <summary>
    /// Call HasDependents with a parameter that has no dependent
    /// </summary>
    [TestMethod]
    public void DependencyGraphHasDependents_TestHasDependentsFalse_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        Assert.IsFalse(graph.HasDependents("A2"));
    }

    [TestMethod]
    public void DependencyGraphHasDependents_TestHasMultipleDependents_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        graph.AddDependency("A1", "A3");
        graph.AddDependency("A1", "A4");
        graph.AddDependency("A1", "B1");
        Assert.IsTrue(graph.HasDependents("A1"));
    }

    // --- Test For HasDependees

    [TestMethod]
    public void DependencyGraphHasDependees_TestHasDependeeEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
        Assert.IsFalse(emptyGraph.HasDependees("A1"));
    }

    [TestMethod]
    public void DependencyGraphHasDependees_TestHasOneDependee_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        Assert.IsTrue(graph.HasDependees("A2"));
    }

    /// <summary>
    /// Call HasDependees with a parameter that has no dependee
    /// </summary>
    [TestMethod]
    public void DependencyGraphHasDependees_TestHasDependeesFalse_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        Assert.IsFalse(graph.HasDependees("A1"));
    }

    [TestMethod]
    public void DependencyGraphHasDependees_TestHasMultipleDependees_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C6", "A2");
        graph.AddDependency("A3", "A2");
        Assert.IsTrue(graph.HasDependees("A2"));
    }

    // --- Test For GetDependents

    /// <summary>
    /// Call GetDependents with a non exist node name will return an empty set of dependents
    /// </summary>
    [TestMethod]
    public void DependencyGraphGetDependents_TestGetDependentsEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
        HashSet<String> emptySet = (HashSet<String>)emptyGraph.GetDependents("A1");
        Assert.AreEqual(0, emptySet.Count());
    }

    [TestMethod]
    public void DependencyGraphGetDependents_TestGetOneDependent_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        HashSet<String> setDependents = (HashSet<String>)graph.GetDependents("A1");
        Assert.AreEqual(1, setDependents.Count());
        Assert.IsTrue(setDependents.Contains("A2"));
    }


    [TestMethod]
    public void DependencyGraphGetDependents_TestGetMultipleDependents_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        graph.AddDependency("A1", "A3");
        graph.AddDependency("A1", "A4");
        HashSet<String> setDependents = (HashSet<String>)graph.GetDependents("A1");
        Assert.AreEqual(3, setDependents.Count());
        Assert.IsTrue(setDependents.Contains("A2"));
        Assert.IsTrue(setDependents.Contains("A3"));
        Assert.IsTrue(setDependents.Contains("A4"));
    }

    // --- Test For GetDependees

    /// <summary>
    /// Call GetDependees with a non exist node name will return an empty set of dependees
    /// </summary>
    [TestMethod]
    public void DependencyGraphGetDependees_TestGetDependeesEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
        HashSet<String> emptySet = (HashSet<String>)emptyGraph.GetDependees("A1");
        Assert.AreEqual(0, emptySet.Count());
    }

    [TestMethod]
    public void DependencyGetDependees_TestGetOneDependee_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "A2");
        HashSet<String> setDependees = (HashSet<String>)graph.GetDependees("A2");
        Assert.AreEqual(1, setDependees.Count());
        Assert.IsTrue(setDependees.Contains("A1"));
    }


    [TestMethod]
    public void DependencyGraphGetDependees_TestGetMultipleDependees_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C4", "A2");
        graph.AddDependency("D9", "A2");
        HashSet<String> setDependees = (HashSet<String>)graph.GetDependees("A2");
        Assert.AreEqual(3, setDependees.Count());
        Assert.IsTrue(setDependees.Contains("B1"));
        Assert.IsTrue(setDependees.Contains("C4"));
        Assert.IsTrue(setDependees.Contains("D9"));
    }

    // --- Test For AddDependency

    [TestMethod]
    public void DependencyGraphAddDependency_TestAddDependencyToEmptyGraph_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C4", "A2");
        graph.AddDependency("D9", "A2");
        graph.AddDependency("A2", "J1");
        graph.AddDependency("A2", "H1");
        graph.AddDependency("A2", "D4");
        Assert.AreEqual(6, graph.Size);
    }

    [TestMethod]
    public void DependencyGraphAddDependency_TestAddDuplicateNodes_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C4", "A2");
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C4", "A2");
        Assert.AreEqual(2, graph.Size);
    }

    // --- Test For RemoveDependency

    [TestMethod]
    public void DependencyGraphRemoveDependency_TestRemoveDependencyEmptyGraph_Valid()
    {
        DependencyGraph emptyGraph = new DependencyGraph();
        Assert.AreEqual(0, emptyGraph.Size);                                                                                                                                                                                                                                                                                                                                                                    
        emptyGraph.RemoveDependency("A1", "A2");
        Assert.AreEqual(0, emptyGraph.Size);
    }

    [TestMethod]
    public void DependencyGraphRemoveDependency_TestRemoveOneDependency_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A2");
        graph.AddDependency("C4", "A2");
        Assert.AreEqual(2, graph.Size);
        Assert.IsTrue(graph.GetDependents("B1").Contains("A2"));
        Assert.IsTrue(graph.GetDependees("A2").Contains("B1"));

        graph.RemoveDependency("B1", "A2");

        Assert.AreEqual(1, graph.Size);
        Assert.IsFalse(graph.GetDependents("B1").Contains("A2"));
        Assert.IsFalse(graph.GetDependees("A2").Contains("B1"));
    }


    [TestMethod]
    public void DependencyGraphRemoveDependency_TestRemoveMultipleDependencies_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A2");
        graph.AddDependency("B1", "A3");
        graph.AddDependency("B1", "A4");
        graph.AddDependency("B1", "A5");
        graph.AddDependency("C4", "A2");
        Assert.AreEqual(5, graph.Size);
        Assert.IsTrue(graph.GetDependents("B1").Contains("A4"));
        Assert.IsTrue(graph.GetDependees("A3").Contains("B1"));
        Assert.IsTrue(graph.HasDependents("C4"));
        

        graph.RemoveDependency("B1", "A4");
        graph.RemoveDependency("B1", "A3");
        graph.RemoveDependency("C4", "A2");

        Assert.AreEqual(2, graph.Size);
        Assert.IsFalse(graph.GetDependents("B1").Contains("A4"));
        Assert.IsFalse(graph.GetDependees("A3").Contains("B1"));
        Assert.IsFalse(graph.HasDependents("C4"));
        Assert.IsTrue(graph.HasDependees("A2"));
    }

    // --- Test For ReplaceDependents
    [TestMethod]
    public void DependencyGraphReplaceDependents_TestReplaceOneDependent_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");
        Assert.IsTrue(graph.GetDependents("A1").Contains("B1"));
        HashSet<string> newDependents = new HashSet<string>();
        newDependents.Add("B2");
        graph.ReplaceDependents("A1", newDependents);
        Assert.IsFalse(graph.GetDependents("A1").Contains("B1"));
        Assert.IsTrue(graph.GetDependents("A1").Contains("B2"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependents_TestReplaceDependentsEmptyGraph_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        HashSet<string> newDependents = new HashSet<string>();
        newDependents.Add("U7");
        newDependents.Add("L1");

        graph.ReplaceDependents("A1", newDependents);
        Assert.AreEqual(2, graph.Size);
        Assert.IsTrue(graph.GetDependents("A1").Contains("U7"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependents_TestReplaceMultipleDependents_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");                                
        graph.AddDependency("A1", "C1");
        Assert.AreEqual(2, graph.Size);        
        
        HashSet<string> newDependents = new HashSet<string>();
        newDependents.Add("B2");
        newDependents.Add("C4");
        newDependents.Add("D1");

        graph.ReplaceDependents("A1", newDependents);

        Assert.AreEqual(3, graph.Size);
        Assert.IsFalse(graph.GetDependents("A1").Contains("B1"));
        Assert.IsTrue(graph.GetDependents("A1").Contains("B2"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependents_TestReplaceWithEmptySetDependents_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");
        graph.AddDependency("A1", "C1");

        Assert.AreEqual(2, graph.Size);
        Assert.IsTrue(graph.HasDependents("A1"));
        Assert.IsTrue(graph.HasDependees("C1"));

        HashSet<string> newDependents = new HashSet<string>();

        graph.ReplaceDependents("A1", newDependents);
        
        Assert.AreEqual(0, graph.Size);
        Assert.IsFalse(graph.HasDependents("A1"));
        Assert.IsFalse(graph.HasDependees("C1"));
    }

    // --- Test For ReplaceDependees
    [TestMethod]
    public void DependencyGraphReplaceDependees_TestReplaceOneDependee_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("A1", "B1");
        Assert.IsTrue(graph.GetDependees("B1").Contains("A1"));
        HashSet<string> newDependees = new HashSet<string>();
        newDependees.Add("B2");
        graph.ReplaceDependees("B1", newDependees);
        Assert.IsFalse(graph.GetDependees("B1").Contains("A1"));
        Assert.IsTrue(graph.GetDependees("B1").Contains("B2"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependees_TestReplaceDependeesEmptyGraph_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        HashSet<string> newDependees = new HashSet<string>();
        newDependees.Add("B7");
        newDependees.Add("K0");
        graph.ReplaceDependees("A1", newDependees);
        Assert.AreEqual(2, graph.Size);
        Assert.IsTrue(graph.HasDependees("A1"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependees_TestReplaceMultipleDependees_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A1");
        graph.AddDependency("C1", "A1");
        Assert.AreEqual(2, graph.Size);

        HashSet<string> newDependees = new HashSet<string>();
        newDependees.Add("B2");
        newDependees.Add("C4");
        newDependees.Add("D1");

        graph.ReplaceDependees("A1", newDependees);

        Assert.AreEqual(3, graph.Size);
        Assert.IsFalse(graph.GetDependees("A1").Contains("B1"));
        Assert.IsTrue(graph.GetDependees("A1").Contains("B2"));
    }

    [TestMethod]
    public void DependencyGraphReplaceDependees_TestReplaceWithEmptySetDependees_Valid()
    {
        DependencyGraph graph = new DependencyGraph();
        graph.AddDependency("B1", "A1");
        graph.AddDependency("C1", "A1");

        Assert.AreEqual(2, graph.Size);
        Assert.IsTrue(graph.HasDependents("B1"));
        Assert.IsTrue(graph.HasDependees("A1"));

        HashSet<string> newDependees = new HashSet<string>();

        graph.ReplaceDependees("A1", newDependees);

        Assert.AreEqual(0, graph.Size);
        Assert.IsFalse(graph.HasDependents("B1"));
        Assert.IsFalse(graph.HasDependees("A1"));
    }


}