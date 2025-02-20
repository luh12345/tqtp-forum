﻿using Insolation.Forum.Api.Models.Auth;
using Insolation.Forum.Api.Services.Jwt;
using Insolation.Forum.Api.Services.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Insolation.Forum.Api.Controllers
{
    [RoutePrefix("api/auth")]
    [EnableCors("*", "*", "*")]
    public class AuthController : BaseController
    {
        private readonly IJwtTokenService jwtService;
        private readonly ILoginService loginService;

        public AuthController(IJwtTokenService jwtService, ILoginService loginService)
        {
            this.jwtService = jwtService;
            this.loginService = loginService;
        }

        /// <summary>
        /// Create an authorize token for valid users
        /// </summary>
        /// <param name="model">User credentials</param>
        /// <returns>Auth token</returns>
        public async Task<HttpResponseMessage> Post([FromBody] AuthModel model)
        {

            IEnumerable<string> roles = await loginService.Login(model.Username, model.Password);
            IEnumerable<string> jwtAudiance = GetAudience();
            if (jwtAudiance == null || !jwtAudiance.Any())
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid or null audiance.");

            string jwtToken = jwtService.CreateToken(model.Username, jwtAudiance.First(), roles);
            return Request.CreateResponse(HttpStatusCode.OK, jwtToken);
        }
    }
}
