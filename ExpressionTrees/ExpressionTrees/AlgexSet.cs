using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExpressionTrees
{
    public class AlgexSet:IDictionary<string, string>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private Dictionary<string, Algex> _dict;

        public bool ImmediateSolve
        {
            get;
            set;
        }

        public AlgexSet()
        {
            _dict = new Dictionary<string, Algex>();
        }

        public bool TrySolveAll()
        {
            bool anySolved = true;
            while (anySolved)
            {
                anySolved = _dict.Keys
                    .Where(x => !_dict[x].IsSolved && _dict[x].DependantOn.Intersect(GetSolvedKeys()).Count() == _dict[x].DependantOn.Count)
                    .Aggregate(false, (current, key) => current || _dict[key].Solve(GetValues()));
            }
            return _dict.Count == _dict.Count(x => x.Value.IsSolved);
        }

        public void SolveAll()
        {
            var allVars = _dict.Aggregate((IEnumerable<string>)(new List<string>()), (current, x) => x.Value.DependantOn.Union(current)).ToList();
            if (allVars.Intersect(_dict.Keys).Count() != allVars.Count) throw new Exception("There are undefined variables");
            if (!TrySolveAll()) throw new Exception("Recursion Detected");
        }

        public void Add(string key, string expression)
        {
            var exp = new Algex(expression);
            this.Add(key, exp);
            if (ImmediateSolve) TrySolveAll();
        }

        private void Add(string key, Algex value)
        {
            _dict.Add(key, value);
            //CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dict.Keys; }
        }

        public bool Remove(string key)
        {
            var toReset = GetAllDependingOn(key);
            if (_dict.Remove(key))
            {
                foreach (var variable in toReset)
                {
                    _dict[variable].Reset();
                }
                //CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
                return true;
            }
            return false;
         }

        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> Values
        {
            get
            {
                return (ICollection<string>)_dict.Select(x => x.Value.InnerExpression);
            }
        }

        public string this[string key]
        {
            get
            {
                return _dict[key].InnerExpression;
            }
            set
            {
                _dict[key].InnerExpression = value;
                foreach (var var in GetAllDependingOn(key))
                {
                    _dict[var].Reset();
                }
                if (ImmediateSolve) TrySolveAll();
                //CollectionChanged.Invoke(_dict[key], new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
            }
        }

        private List<string> GetAllDependingOn(string key)
        {
            var result = new List<string>{key};
            var extendedResult = new List<string>{key};
            do
            {
                result = result.Union(extendedResult).ToList();
                var d = _dict.Where(x => x.Value.DependantOn.Intersect(result).Count() > 0).Select(x => x.Key).ToList();
                extendedResult = extendedResult.Union(d).ToList();
            }
            while (result.Count() < extendedResult.Count());
            result.Remove(key);
            result.Sort();
            return result;
        }

        public Dictionary<string, double> GetValues()
        {
            var result = new Dictionary<string, double>();
            foreach (var key in _dict.Keys)
            {
                if (_dict[key].IsSolved)
                    result.Add(key, _dict[key].Value);
            }
            return result;
         }

        public List<string> GetSolvedKeys()
        {
            return _dict.Where(x => x.Value.IsSolved).Select(x => x.Key).ToList();
        }

        public void RenameVariable(string oldName, string newName)
        {

            if (!_dict.ContainsKey(oldName)) throw new KeyNotFoundException();
            if (_dict.ContainsKey(newName)) throw new ArgumentException();
            var item = _dict[oldName];
            foreach (var var in GetAllDependingOn(oldName))
            {
                    _dict[var].Reset();
            }
            foreach (var var in _dict.Where(x => x.Value.DependantOn.Contains(oldName)))
            {
                _dict[var.Key].InnerExpression = _dict[var.Key].InnerExpression.Replace("{"+oldName+"}", "{"+newName+"}"); 
            }
            _dict.Remove(oldName);
            Add(newName, item.InnerExpression);
        }

        public double Value(string key)
        {
            return _dict[key].Value;
        }

        public void Add(KeyValuePair<string, string> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dict.Clear();
            //CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dict[item.Key].InnerExpression == item.Value;
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            if (!Contains(item)) return false;
            return Remove(item.Key);
            
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
