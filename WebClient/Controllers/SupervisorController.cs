//
// SupervisorController.cs
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
using AkkaDotNetCoreDocker.BoundedContexts.MaintenanceBilling.Aggregates;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class SupervisorController : Controller
    {
        [Route("api/supervisor")]
        [HttpGet]
        public async Task<SupervisedAccounts> AccountSupervisor()
        {
            var answer = await ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor")
                .Ask<ThisIsMyStatus>(new TellMeYourStatus(), TimeSpan.FromSeconds(5));

            var response = new SupervisedAccounts(answer.Message, answer.Accounts);

            return response;
        }

        [HttpGet]
        [Route("api/supervisor/run")]
        public SupervisedAccounts StartAccounts()
        {
            var answer = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor")
                .Ask<ThisIsMyStatus>(new StartAccounts(), TimeSpan.FromSeconds(30)).Result;
            var response = new SupervisedAccounts(answer.Message, answer.Accounts);

            return response;
        }

        [HttpPost]
        [Route("api/supervisor/simulation")]
        public string BoardAccounts([FromBody]SimulateBoardingOfAccountModel client)
        {
            var answer = ActorSystemRefs
                .ActorSystem
                .ActorSelection($"akka://demo-system/user/demo-supervisor")
                .Ask<string>(new SimulateBoardingOfAccounts(
                     client.ClientName,
                     client.ClientAccountsFilePath,
                     client.ObligationsFilePath   
                ), TimeSpan.FromSeconds(5)).Result;
            return answer;
        }

    }
}
