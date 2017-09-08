//
// PaymentController.cs
//
// Author:
//       Alfredo Herrera <alfredherr@gmail.com>
//
// Copyright (c) 2017 Alfrdo Herrera
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Events;
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Commands;
using Microsoft.AspNetCore.Mvc;
using WebClient.ActorManagement;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class AccountController : Controller
    {
        [Route("api/account")]
        [HttpGet]
        public string Account()
        {
            return "Which account are you looking for?";
        }

       
        [Route("api/account/{actorName}/info")]
        [HttpGet]
         public async Task<AccountStateViewModel> AccountDetails(string actorName)
        {
            var system = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor/{actorName}").ResolveOne(TimeSpan.FromSeconds(1));
            if (system.Exception != null)
            {
                return new AccountStateViewModel($"{actorName} is not running at the moment");
            }
            var response = await system.Result.Ask<ThisIsMyInfo>(new TellMeYourInfo(), TimeSpan.FromSeconds(1));

            return  new AccountStateViewModel(response.Info);
        }

        [Route("api/account/{actorName}")]
        [HttpGet]
        public async Task<string> Account(string actorName)
        {
            var system = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor/{actorName}").ResolveOne(TimeSpan.FromSeconds(1));
            if (system.Exception != null)
            {
                return $"{actorName} is not running at the moment";
            }
            var response = await system.Result.Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(1));
            return response.Message;
        }

    }
}
