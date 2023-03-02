﻿using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Support;
using DigitalLearningSolutions.Data.ViewModels.UserCentreAccount;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.Login;
using DigitalLearningSolutions.Web.ViewModels.UserCentreAccounts;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
  public class UsersControllerTests
  {
    private UsersController controller = null!;

    private IUserService userService = null!;
    private IUserCentreAccountsService userCentreAccountsService = null!;
    [SetUp]
    public void Setup()
    {
      userService = A.Fake<IUserService>();
      userCentreAccountsService = A.Fake<IUserCentreAccountsService>();
      controller = new UsersController(userCentreAccountsService, userService);
      A.CallTo(() => userService.GetUnverifiedEmailsForUser(10));
    }
    [Test]
    public void Users_page_should_return_expected_user_centre_account_view_page()
    {
      // When
      var results = controller.CentreAccounts(10);

      // Then
      results.Should().BeViewResult().WithViewName("UserCentreAccounts");
    }
    [Test]
    public void Users_page_should_return_expected_user_centre_account()
    {
      // Given
      var userEntity = userService.GetUserById(10);
      var UserCentreAccountsRoleViewModel =
         userCentreAccountsService.GetUserCentreAccountsRoleViewModel(userEntity);
      // Then
      using (new AssertionScope())
      {

        A.CallTo(
                () => userCentreAccountsService.GetUserCentreAccountsRoleViewModel(
                            userEntity
                        )
                )

          .MustHaveHappened();
      }

    }
  }
}
