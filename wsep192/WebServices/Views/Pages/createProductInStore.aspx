﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="createProductInStore.aspx.cs" Inherits="WebServices.Views.Pages.createProductInStore" %>

<asp:Content ID="content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--================Login Box Area =================-->
    <section class="login_box_area section-margin">
        <div class="container">
            <div class="row">
                <div class="col-lg-6">
                    <div class="login_box_img">
                        <div class="hover">
                            <h4>Doesn't have a store yet?</h4>
                            <p>You can open a store in our website in a few minutes, and start manage your own store!</p>
                            <a class="button button-account" href="/LoginUser">Open store now</a>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="login_form_inner register_form_inner">
                        <h3>Create Product In Store</h3>
                        <form class="row login_form" action="#/" id="createProductInStore_form">
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="productName" name="name" placeholder="Product Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Product Name'">
                            </div>
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="productCategory" name="productCategory" placeholder="Product Category" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Product Category'">
                            </div>

                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="productDetail" name="productDetail" placeholder="Product Detail" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Product Detail'">
                            </div>
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="productPrice" name="productPrice" placeholder="Product Price" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Product Price'">
                            </div>
                            <div class="col-md-12 form-group">
                                <input type="text" class="form-control" id="storeName" name="storeName" placeholder="Store Name" onfocus="this.placeholder = ''" onblur="this.placeholder = 'Store Name'">
                            </div>

                            <small id="createProductInStoreAlert" class="form-text text-muted text-Alert"></small>

                            <div class="col-md-12 form-group">
                                <input type="button" class="button button-register w-100" id="addProductInStoreAlert" value="Create Product"></>
                            </div>

                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <!--================End Login Box Area =================-->
</asp:Content>
