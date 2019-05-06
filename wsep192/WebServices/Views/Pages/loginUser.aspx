﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="loginUser.aspx.cs" Inherits="WebServices.Views.Pages.loginUser" %>

<asp:Content ID="content1" ContentPlaceHolderID="MainContent" runat="server">

    <!--================Login Box Area =================-->
    <section class="login_box_area section-margin">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <div class="login_box_img">
                        <div class="hover">
                            <h4>New to our website?</h4>
                            <p>There are advances being made in science and technology everyday, and a good example of this is the</p>
                            <a class="button button-account" href="/RegisterUser">Create an Account</a>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="login_form_inner">
                        <h3>Log in to enter</h3>
                        <form class="row login_form" action="#/" id="contactForm">
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="username" name="name" placeholder="Username" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Username'">
                            </div>
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="password" name="name" placeholder="Password" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Password'">
                            </div>

                            <small id="loginAlert" class="form-text text-muted text-Alert"></small>


                            <div class="col-md-12 form-group">
                                <input type="button" class="button button-login w-100" id="loginButton" value="Login"></>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--================End Login Box Area =================-->
    <script type="text/javascript">

        $(document).ready(function () {
            $("#loginButton").click(function () {
                var myCookie = getCookie("LoggedUser");
                if (myCookie == null) {
                    username = $("#username").val();
                    pass = $("#password").val();

                    jQuery.ajax({
                        type: "GET",
                        url: baseUrl + "/api/user/LoginUser?username=" + username + "&password=" + pass,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response == "User successfuly logged in") {
                                document.cookie = "LoggedUser=" + username
                                alert(response);
                                window.location.href = baseUrl + "/";
                            }
                            else {
                                $("#loginAlert").html('Failure - ' + response);
                            }
                        },
                        error: function (response) {
                            console.log(response);
                            window.location.href = baseUrl + "/error";
                        }
                    });
                }
                else
                    $("#loginAlert").html('Failure - ' + " already logged in");

            });
        });

    </script>

</asp:Content>
