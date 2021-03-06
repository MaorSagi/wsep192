﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="assignOwner.aspx.cs" Inherits="WebServices.Views.Pages.assignOwner" %>

<asp:Content ID="content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--================Login Box Area =================-->
    <section class="login_box_area section-margin">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <div class="login_box_img">
                        <div class="hover">
                            <h4>Don't forget</h4>
                            <p>Only owner can assign new owners.</p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="login_form_inner register_form_inner">
                        <h3>Assign New Owner</h3>
                        <form class="row login_form" action="#/" id="register_form">

                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="userToAssign" name="userToAssign" placeholder="New Owner Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'userToAssign'">
                            </div>
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="storeName" name="storeName" placeholder="Store Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'storeName'">
                            </div>
                            <input type="button" id="assignOwnerButton" value="Assign Owner" class="button button-login w-100"></>

                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--================End Login Box Area =================-->
    <script type="text/javascript">
        $(document).ready(function () {
            $("#assignOwnerButton").click(function () {
                var ownerName = getCookie("LoggedUser");
                if (ownerName != null) {
                    userToAssign = $("#userToAssign").val();
                    storeName = $("#storeName").val();
                    jQuery.ajax({
                        type: "GET",
                        url: baseUrl + "/api/user/assignOwner?ownerName=" + ownerName + "&userToAssign=" + userToAssign + "&storeName=" + storeName,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response != "false") {
                                document.cookie = "LoggedUser=" + ownerName
                                alert(response);
                                window.location.href = baseUrl + "/";
                            }
                            else {
                                alert("Owner cannot be assigned. The user does not exists or loggedin user cannot perform this act.");
                            }
                        },
                        error: function (response) {
                            alert("Owner cannot be assigned. The user does not exists or loggedin user cannot perform this act.");
                        }
                    });
                }
                else
                    alert("The user is not an owner");
            });
        });
    </script>
</asp:Content>
