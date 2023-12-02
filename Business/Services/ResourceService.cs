using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IResourceService
    {
        IQueryable<ResourceModel> Query();
        Result Add(ResourceModel model);
        Result Update(ResourceModel model);
        Result Delete(int id);
        public ResourceModel GetItem(int id);

        List<ResourceModel> GetList();
    }
    public class ResourceService : IResourceService
    {
        private readonly Db _db;

        public ResourceService(Db db)
        {
            _db = db;
        }

        public IQueryable<ResourceModel> Query()
        {
          return  _db.Resources.Include(r => r.UserResources).Select(r => new ResourceModel()
                  {
                      Content = r.Content,
                      Date = r.Date,
                      Id = r.Id,
                      Score = r.Score,
                      Title = r.Title,

                      //ScoreOutput = r.Score.ToString("N1", new CultureInfo("en-US"))
                      ScoreOutput = r.Score.ToString("N1"),
                      DateOutput = r.Date.HasValue ? r.Date.Value.ToString("MM/dd/yyyy  hh:mm:ss") : "",

                      UserCountOutput = r.UserResources.Count,
                      UserNamesOutput = string.Join("<br/>", r.UserResources.Select(ur => ur.User.UserName)),
                      UserIdsInput = r.UserResources.Select(ur => ur.UserId).ToList(),          

                  }).OrderByDescending(r => r.Date).ThenByDescending(r => r.Score);
        }

        public Result Add(ResourceModel model)
        {
            if (model.Date.HasValue &&
                 _db.Resources.Any(r => (r.Date ?? new DateTime()).Date == model.Date.Value.Date &&
                 r.Title.ToUpper() == model.Title.ToUpper().Trim()))
                return new ErrorResult("Resource with the same title and date exists!");
            var entity = new Resource()
            {
                Content = model.Content?.Trim(),
                Date = model.Date,
                Score = model.Score ?? 0,
                Title = model.Title.Trim(),

                //inserting many to many relation entity
                UserResources = model.UserIdsInput?.Select(userId => new UserResource()
                {
                    UserId = userId
                }).ToList()
            };
            _db.Resources.Add(entity);
            _db.SaveChanges();
            return new SuccessResult("Resource added successfuly");
        }

        public Result Update(ResourceModel model)
        {
            if (model.Date.HasValue &&
                _db.Resources.Any(r => (r.Date ?? new DateTime()).Date == model.Date.Value.Date &&
                r.Title.ToUpper() == model.Title.ToUpper().Trim() && r.Id != model.Id))
                return new ErrorResult("Resource with the same title and date exists!");

            // deleting many to many relational entity
            var existingEntity = _db.Resources.Include(r => r.UserResources).SingleOrDefault(r => r.Id == model.Id);
            if (existingEntity is not null && existingEntity.UserResources is not null)
                _db.UserResources.RemoveRange(existingEntity.UserResources);

            // existingEntity queried from the database must be updated since we got the existingEntity
            // first as above, therefore changes of the existing entity are being tracked by Entity Framework,
            // if disabling of change tracking is required, AsNoTracking method must be used after the DbSet,
            // for example _db.Resources.AsNoTracking()
            existingEntity.Content = model.Content?.Trim();
            existingEntity.Date = model.Date;
            existingEntity.Score = model.Score ?? 0;
            existingEntity.Title = model.Title.Trim();

            // inserting many to many relational entity
            existingEntity.UserResources = model.UserIdsInput?.Select(userId => new UserResource()
            {
                UserId = userId
            }).ToList();

            _db.Resources.Update(existingEntity);
            _db.SaveChanges(); // changes in all DbSets are commited to the database by Unit of Work

            return new SuccessResult("Resource updated successfully.");
        }


        public Result Delete(int id)
        {
            var entity = _db.Resources.Include(r => r.UserResources).SingleOrDefault(r => r.Id == id);

            if (entity == null)
                 return new ErrorResult("Resource not found");
          
            _db.UserResources.RemoveRange(entity.UserResources);
            _db.Resources.Remove(entity);
            _db.SaveChanges();
            return new SuccessResult("Resource deleted successfuly");
        }

        public List<ResourceModel> GetList()
        {
            return Query().ToList();
        }

        public ResourceModel GetItem(int id) => Query().SingleOrDefault(r => r.Id == id);
    }
}
