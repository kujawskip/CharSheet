using System;
using System.Collections.Generic;
using ExpressionTrees;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void ExpressionBasicTests()
        {
            var tests = new[] 
            {
                new Tuple<string, double>("3", 3),
                new Tuple<string, double>("2+2", 4),
                new Tuple<string, double>("2/2", 1),
                new Tuple<string, double>("(2+2)", 4),
                new Tuple<string, double>("2*2", 4),
                new Tuple<string, double>("2+2*2", 6),
                new Tuple<string, double>("(2+2)*2", 8),
                new Tuple<string, double>("(2+2)*(2+2)", 16),
                new Tuple<string, double>("(20+2)/11", 2)
            };
            foreach (var test in tests)
            {
                var exp = new Algex(test.Item1, null, true);
                Assert.AreEqual(test.Item2, exp.Value);
            }
        }
        [TestMethod]
        public void ExpressionAdvancedTests()
        {
            var tests = new[] 
            {
                new Tuple<string, double>("2+-2", 0),
                new Tuple<string, double>("-1*2", -2),
                new Tuple<string, double>("-1*-2", 2),
                new Tuple<string, double>("-3*(2+2)", -12),
                new Tuple<string, double>("-(1-2)", 1)
            };
            foreach (var test in tests)
            {
                var exp = new Algex(test.Item1, null, true);
                Assert.AreEqual(test.Item2, exp.Value);
            }
        }

        [TestMethod]
        public void ExpressionVariableContextTests()
        {
            var var = new Dictionary<string, double> {{"a", 2}, {"b", -1}, {"cd", 2}, {"zero", 0}};
            var tests = new[] 
            {
                new Tuple<string, double>("{a}+-2", 0),
                new Tuple<string, double>("{b}*2", -2),
                new Tuple<string, double>("-1*-2", 2),
                new Tuple<string, double>("-3*({cd}+2)", -12),
                new Tuple<string, double>("-(1-2)+{zero}", 1)
            };
            foreach (var test in tests)
            {
                var exp = new Algex(test.Item1, var, true);
                Assert.AreEqual(test.Item2, exp.Value);
            }
        }

        [TestMethod]
        public void ExpressionVariableContextNestedTests()
        {
            var var = new Dictionary<string, double> {{"a", 2}, {"b", -1}, {"cd", 5}, {"zero", 0}};
            var tests = new[] 
            {
                new Tuple<string, double>("(2+3*(5-(7+12))*{zero})", 2),
                new Tuple<string, double>("(3-3)/(((3+3)*5)*{b})*(-1*{b})", 0),
                new Tuple<string, double>("0*(3-(10*(2+2)/({a}*10)))+10", 10)
            };
            foreach (var test in tests)
            {
                var exp = new Algex(test.Item1, var, true);
                Assert.AreEqual(test.Item2, exp.Value);
            }
        }
        [TestMethod]
        public void ExpressionSetTest()
        {
            var tests = new[] 
            {
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("d","{a}*{c}", 6),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30)
            };
            var set = new AlgexSet();
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            set.SolveAll();
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }

        [TestMethod]
        public void ExpressionSetTestImmediateSolve()
        {
            var tests = new[] 
            {
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("d","{a}*{c}", 6),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30)
            };
            var set = new AlgexSet {ImmediateSolve = true};
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }
        [TestMethod]
        public void ExpressionSetTestImmediateSolveReverseInput()
        {
            var tests = new List<Tuple<string, string, double>>
            {
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("d","{a}*{c}", 6),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30)
            };
            tests.Reverse();
            var set = new AlgexSet {ImmediateSolve = true};
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }
        [TestMethod]
        public void ExpressionSetTestImmediateSolveRandomizedInput()
        {
            var tests = new List<Tuple<string, string, double>>
            {
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("d","{a}*{c}", 6)
            };
            var set = new AlgexSet {ImmediateSolve = true};
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }

        [TestMethod]
        public void ExpressionSetTestRenameVariable()
        {
            var tests = new List<Tuple<string, string, double>>
            {
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("d","{a}*{c}", 6),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30)
            };
            var set = new AlgexSet {ImmediateSolve = true};
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            set.RenameVariable("c", "cc");
            tests[2] = new Tuple<string, string, double>("cc", "{a}+{b}", 3);
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }

        [TestMethod]
        public void ExpressionSetTestRemoveVariable()
        {
            var tests = new List<Tuple<string, string, double>>
            {
                new Tuple<string, string, double>("a", "2", 2),
                new Tuple<string, string, double>("b", "{a}-1", 1),
                new Tuple<string, string, double>("c","{a}+{b}", 3),
                new Tuple<string, string, double>("d","{a}*{c}", 6),
                new Tuple<string, string, double>("e","{d}-{b}", 5),
                new Tuple<string, string, double>("f","{e}+4*{b}", 9),
                new Tuple<string, string, double>("g","{f}/{c}+1", 4),
                new Tuple<string, string, double>("all","({f}/{c}-{g}+{b})*({a}+{f})+{d}*{e}", 30)
            };
            var set = new AlgexSet {ImmediateSolve = true};
            foreach (var test in tests)
            {
                set.Add(test.Item1, test.Item2);
            }
            set.Remove("a");
            tests.RemoveAt(0);
            var results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(false, results.ContainsKey(test.Item1));
            }
        
            set.Add("a", "2");
            results = set.GetValues();
            foreach (var test in tests)
            {
                Assert.AreEqual(results[test.Item1], test.Item3);
            }
        }

    }
}
