﻿using Core.Dto;
using Core.Services.Base;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Admin
{
    public abstract class AbstractAdminController<W, R> : AbstractQueryableCrudController<W, R>
        where W : Entity where R : IIdentified
    {
        protected AbstractAdminController(CrudService<W, R> service) : base(service)
        {
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("all")]
        public override IActionResult All([FromQuery] int skip = 0, [FromQuery] int take = 1000)
        {
            return base.All(skip, take);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("")]
        public override IActionResult Index([FromQuery] PageQuery pageQuery)
        {
            return base.Index(pageQuery);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("{slug}")]
        public override IActionResult Show(string slug)
        {
            return base.Show(slug);
        }
    }

    public abstract class AbstractAdminController<T> : AbstractAdminController<T, T> where T : Entity
    {
        protected AbstractAdminController(CrudService<T, T> service) : base(service)
        {
        }
    }
}