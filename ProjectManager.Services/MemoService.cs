using System;
using System.Collections.Generic;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;
using ProjectManager.DAL.Repositories;
using ProjectManager.Models;
using ProjectManager.Models.Xero;
using ProjectManager.Services.XeroService.Payroll;

namespace ProjectManager.Services
{
    public class MemoService
    {
        private readonly IRepository<Memo> _memoRepository;
        private readonly IRepository<Employee> _employeeRepository = new EmployeeRepository(new DataContext()); 

        public MemoService(IRepository<Memo> memoRepository)
        {
            this._memoRepository = memoRepository;
        }

        public void CreateMemo(string note, Guid xeroEmployeeId)
        {
            var employee = EmployeeService
                .GetAllEmployees()
                .FirstOrDefault(e => e.XeroEmployeeId == xeroEmployeeId);

            if (employee == null)
                throw new InvalidOperationException("Employee Not Found");

            var employeeFromDataBase = _employeeRepository.GetAll()
                .SingleOrDefault(e => e.XeroEmployeeId == xeroEmployeeId);
            
            var memo = new Memo()
            {
                DateTime = DateTime.Now,
                Notes = note
            };

            if (employeeFromDataBase != null)
                memo.EmployeeId = employeeFromDataBase.EmployeeId;

            _memoRepository.Insert(memo);
            _memoRepository.Commit();
        }

        public Memo RetrieveMemo(int memoId)
        {
            var memo = _memoRepository.GetById(memoId);

            return memo;
        }

        public IList<Memo> GetAllMemos()
        {
            return _memoRepository
                .GetAll()
                .ToList();
        }

        public void DeleteMemo(int memoId)
        {
            _memoRepository.Delete(memoId);
            _memoRepository.Commit();
        }
    }
}
