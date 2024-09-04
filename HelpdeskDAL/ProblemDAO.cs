using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskDAL
{
    public class ProblemDAO
    {
        readonly IRepository<Problem> _repo;
        public ProblemDAO()
        {
            _repo = new HelpdeskRepository<Problem>();
        }

        public async Task<Problem?> GetByDescription(string desc)
        {
            Problem? selectedProblem;
            try
            {
                //HelpdeskContext _db = new HelpdeskContext();
                selectedProblem = await _repo.GetOne(prb => prb.Description == desc);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedProblem;
        }

        public async Task<List<Problem>> GetAll()
        {
            List<Problem> allProblems;
            try
            {
                //HelpdeskContext _db = new();
                allProblems = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allProblems;
        }

        public async Task<int> Add(Problem newProblem)
        {
            try
            {
                await _repo.Add(newProblem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return newProblem.Id;
        }

        public async Task<UpdateStatus> Update(Problem updatedProblem)
        {
            UpdateStatus status;
            try
            {
                status = await _repo.Update(updatedProblem);
            }
            catch (DbUpdateConcurrencyException)
            {
                status = UpdateStatus.Stale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return status;
        }

        public async Task<int> Delete(int? id)
        {
            int problemsDeleted = -1;
            try
            {
                problemsDeleted = await _repo.Delete((int)id!); // returns # of rows removed
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return problemsDeleted;
        }
    }
}
