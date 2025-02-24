﻿using System.Linq;
using System.Linq.Dynamic.Core;
using Core.Dto;
using Core.Services.Base;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public abstract class AbstractCrudController<W, R> : Controller where W : Entity where R : IIdentified
    {
        protected readonly CrudService<W, R> Service;

        protected AbstractCrudController(CrudService<W, R> service)
        {
            Service = service;
        }

        [HttpGet]
        [Route("all")]
        public virtual IActionResult All([FromQuery] int skip = 0, [FromQuery] int take = 1000)
        {
            return new JsonResult(
                Service.List(query => Query(query)
                    .OrderBy(string.Join(",", Service.DefaultSort))
                    .Skip(skip)
                    .Take(take)
                )
            );
        }

        [HttpGet]
        [Route("")]
        public virtual IActionResult Index([FromQuery] PageQuery pageQuery)
        {
            return new JsonResult(Service.Page(pageQuery, Query));
        }

        protected abstract IQueryable<W> Query(IQueryable<W> query);

        [HttpGet]
        [Route("{slug}")]
        public virtual IActionResult Show(string slug)
        {
            return new JsonResult(Service.Get(slug));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("")]
        public virtual IActionResult Store([FromBody] W entity)
        {
            return new JsonResult(Service.Create(entity));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{slug}")]
        public virtual IActionResult Update(string slug, [FromBody] W entity)
        {
            return new JsonResult(Service.Update(slug, entity));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{slug}")]
        public virtual IActionResult Destroy(string slug)
        {
            Service.Delete(slug);
            return new OkResult();
        }
    }
}