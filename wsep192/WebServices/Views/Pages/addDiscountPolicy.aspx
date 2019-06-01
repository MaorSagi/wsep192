﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="addDiscountPolicy.aspx.cs" Inherits="WebServices.Views.Pages.addDiscountPolicy" %>

<asp:Content ID="content1" ContentPlaceHolderID="MainContent" runat="server">

    <!--================Checkout Area =================-->
    <section class="checkout_area section-margin--small">
        <div class="container">

            <div class="billing_details" id="ship_and_pay_div">
                <div class="row">
                    <div class="col-lg-8">

                        <h3>Discount polices</h3>
                        <form class="row contact_form" action="#" method="post" novalidate="novalidate">
                            <div class="col-md-12 form-group p_star">
                                <select class="policy_select" id="value">
                                    <option value="0">Revealed discount</option>
                                    <option value="1">Conditional discount</option>
                                </select>
                            </div>
                            <div class="text-center">
                                <a class="button button-paypal" id="policyButton" href="#">Select policy</a>
                            </div>
                        </form>


                        <div id="policyDiv" style="visibility: hidden">
                            <h3 style="padding-top: 30px;">Policy Details</h3>
                            <form class="row contact_form" action="#" id="policyDetails" method="post" novalidate="novalidate"></form>

                        </div>
                        <div class="text-center"><a class="button button-paypal" style="visibility: hidden" id="addDiscountButton">Add Discount Policy</a></div>

                    </div>
                </div>
            </div>

        </div>
    </section>
    <!--================End Checkout Area =================-->

    <script type="text/javascript">
        $(document).ready(function () {

            $("#policyButton").click(function () {
                var userName = getCookie("LoggedUser");
                if (userName != null) {
                    var details;
                    var type = $('#value').val();
                    document.getElementById("policyDetails").remove();
                    alert("Not finished yet");

                    if (type == 0) {
                        details = '<form class="row contact_form" action="#" id="policyDetails" method="post" novalidate="novalidate">'
                            + '<div class="col-md-12 form-group p_star"><div class="col-md-6 form-group p_star"><input type="text" class="form-control" id="product" placeholder="Product name"><span class="placeholder" data-placeholder="Product name"></span> </div> </div>'
                            + '<div class="col-md-12 form-group p_star"><div class="col-md-6 form-group p_star"><input type="text" class="form-control" id="store" placeholder="Store name"><span class="placeholder" data-placeholder="Store name"></span> </div> </div>'
                            + '<div class="col-md-12 form-group p_star"><div class="col-md-6 form-group p_star"><input type="text" class="form-control" id="discount" placeholder="Discount percentage"><span class="placeholder" data-placeholder="Discount percentage"></span> </div> </div>'
                            + '<div class="col-md-12 form-group p_star"><div class="col-md-6 form-group p_star"><input type="text" class="form-control" id="expiredDate" placeholder="Num of Days to be expired"><span class="placeholder" data-placeholder="Num of Days to be expired"></span> </div> </div>'
                            + '<div class="col-md-12 form-group p_star"><div class="col-md-6 form-group p_star"><input type="text" class="form-control" id="logic" placeholder="Logical condition"><span class="placeholder" data-placeholder="Logical condition"></span> </div> </div>'
                            + '</form>'
                    }
                    if (type == 1) {
                        
                    }

                    $('#policyDiv').append(details);
                    document.getElementById("policyDiv").style.visibility = "visible";
                    document.getElementById("addDiscountButton").style.visibility = "visible";
                    
                }
                else
                    alert("User isn't logged in");


            });

            $("#addDiscountButton").click(function () {
                var userName = getCookie("LoggedUser");
                if (userName != null) {
                    type = $("#value").val();
                    store = $("#store").val();
                    var products = '',discount = '',expiredDate = '',logic = '';
                    if (type == 0) {
                        discount = $("#discount").val();
                        expiredDate = $("#expiredDate").val();
                        logic = $("#logic").val();
                    }
                    if (type == 1) {

                    }

                    jQuery.ajax({
                        type: "GET",
                        url: baseUrl + "/api/store/AddDiscountPolicy?products="
                            + products + "&discount=" + discount + "&expiredDate=" + expiredDate
                            + "&logic=" + logic + "&store=" + store + "&user=" + userName,

                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            if (response == 'success') {
                                alert("Policy has created successfully");
                                window.location.href = baseUrl + "/";
                            }
                            else
                                alert(response);
                        },
                        error: function (response) {
                            alert('Error in addDiscountPolicy');
                        }
                    });
                }
                else
                    alert("User isn't logged in");


            });
        });

    </script>

</asp:Content>
