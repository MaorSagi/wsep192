﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="assignManager.aspx.cs" Inherits="WebServices.Views.Pages.assignManager" %>

<asp:Content ID="content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--================Login Box Area =================-->
     <section class="login_box_area section-margin">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <div class="login_box_img">
                        <div class="hover">
                            <h4>Don't forget</h4>
                            <p>Only owner can assign new managers.</p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="login_form_inner register_form_inner">
            <h3>Assign New Manager</h3>
            <form class="row login_form" action="#/" id="register_form">

                <div class="col-md-12 form-group">
                    <input type="text" class="form-control" id="userToAssign" name="userToAssign" placeholder="New Manager Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'userToAssign'">
                </div>
                <div class="col-md-12 form-group">
                    <div>
                        <input type="text" class="form-control" id="storeName" name="storeName" placeholder="Store Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'storeName'">
                    </div>
                    <div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per1" value="AddDiscountPolicy">
                            <label class="custom-control-label" for="per1">Add Discount Policy</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per2" value="AddPurchasePolicy">
                            <label class="custom-control-label" for="per2">Add Purchase Policy</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per3" value="CreateNewProductInStore">
                            <label class="custom-control-label" for="per3">Create New Product In Store</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per4" value="AddProductsInStore">
                            <label class="custom-control-label" for="per4">Add Products In Store</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per5" value="RemoveProductsInStore">
                            <label class="custom-control-label" for="per5">Remove Products In Store</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per6" value="EditProductInStore">
                            <label class="custom-control-label" for="per6">Edit Product In Store</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per7" value="CommunicationWithCustomers">
                            <label class="custom-control-label" for="per7">Communication With Customers</label>
                        </div>
                        <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="per8" value="PurchasesHistory">
                            <label class="custom-control-label" for="per8">Purchases History</label>
                        </div>
                    </div>
                </div>
                <input type="button" id="assignManagerButton" value="Assign Manager" class="button button-login w-100"></>

            </form>
        </div>
            </div>
        </div>
            </div>
    </section>
    <!--================End Login Box Area =================-->
    <script type="text/javascript">
        $(document).ready(function () {
            $("#assignManagerButton").bind("click", function (e) {
                var permissions = "";
                $(".custom-control-input").each(function (e) {
                    if ($(this).is(':checked'))
                        permissions = permissions + $(this).attr('value') + " ";
                })
                var ownerName = getCookie("LoggedUser");
                if (ownerName != null) {
                    userToAssign = $("#userToAssign").val();
                    storeName = $("#storeName").val();
                    console.log(baseUrl + "/api/user/assignManager?ownerName=" + ownerName + "&userToAssign=" + userToAssign + "&storeName=" + storeName + "&permissions=" + permissions
                    );
                    jQuery.ajax({
                        type: "GET",
                        url: baseUrl + "/api/user/assignManager?ownerName=" + ownerName + "&userToAssign=" + userToAssign + "&storeName=" + storeName + "&permissions=" + permissions,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response == "Manager successfuly was assigned") {
                                document.cookie = "LoggedUser=" + ownerName
                                alert(response);
                                window.location.href = baseUrl + "/";

                            }
                            else {
                                alert('Manager cannot be assigned. The user does not exists or loggedin user cannot perform this act.');
                            }
                        },
                        error: function (response) {
                            alert('Manager cannot be assigned. The user does not exists or loggedin user cannot perform this act.');
                        }
                    });
                }
                else
                    alert('User is not an owner or a premissioned manager to preform this act.');
            });
        });


    </script>
</asp:Content>
