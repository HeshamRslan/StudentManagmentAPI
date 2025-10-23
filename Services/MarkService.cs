using StudentManagmentAPI.Models;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services
{
    public class MarkService
    {
        private readonly ConcurrentDictionary<int, Mark> _marks = new();
        private int _idCounter = 1;

        public IEnumerable<Mark> GetAll() => _marks.Values;

        public Mark? GetById(int id)
            => _marks.TryGetValue(id, out var mark) ? mark : null;

        public bool Add(Mark mark)
        {
            mark.Id = _idCounter++;
            return _marks.TryAdd(mark.Id, mark);
        }

        public bool Update(int id, Mark updatedMark)
        {
            if (!_marks.ContainsKey(updatedMark.Id))
                return false; 
            
            _marks[updatedMark.Id] = updatedMark;
            return true;
        }

        public bool Remove(int id)
            => _marks.TryRemove(id, out _);
    }
}
