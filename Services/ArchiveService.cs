using System.Collections.Concurrent;
using System.Collections.Generic;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly ConcurrentDictionary<int, Class> _archived = new();
        public IEnumerable<Class> GetAll() => _archived.Values;
        public bool Archive(Class c) => _archived.TryAdd(c.Id, c);
    }
}
