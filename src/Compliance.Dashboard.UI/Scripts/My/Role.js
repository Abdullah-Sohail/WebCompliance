var currentPage = 1;
var sortAsc = true;
var timer;

function init(data) {
    if (data) {
        furnishDatatable(data);
    }

    $(".searchRoles").keydown(function () {
        clearTimeout(timer);  //clear any running timeout on key up
        timer = setTimeout(function () { //then give it a second to see if the user is finished
            getRoles();
        }, 500);
    });


    $("#page").click(function () {
        return false;
    });

    $('body').on('click', '.createRole', function () {
        createRole(this);
    });

    $('body').on('click', '.editRole', function () {
        editRole(this);
    });

    $('body').on('click', '.deleteRole', function () {
        deleteRole(this);
    });

}

function sortRoles(field, column) {
    $("#role-data-table thead th span").attr('class', 'pull-right icon-sort');
    var icon = !sortAsc ? "icon-sort-up" : "icon-sort-down";
    $(column).find("span").removeClass("icon-sort").addClass(icon);
    sortAsc = !sortAsc;
    getRoles(currentPage);
}

function loader(status) {
    if (status == 'show') {
        $('#content').append('<div class="loading">Loading&#8230;</div>');
    }
    else {
        $('div.loading').remove();
    }
}

function getRoles(page) {
    if (!page) {
        page = 1;
    }


    var search = $(".searchRoles").val();
    var url = "/Role/Index?page=" + page + "&search=" + search + "&sortField=Name&isAsc=" + sortAsc

    if (search == "")
        loader('show');

    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (response) {
            loader('hide');
            furnishDatatable(response)
        },
        failure: function (response) {
            loader('hide');
            console.log(response);
        },
        error: function (response) {
            loader('hide');
            console.log(response);
        }
    });
}

function furnishDatatable(data) {
    currentPage = data.Pager.CurrentPage;
    $("#role-data-table tbody").empty();

    $('#userRowTmpl')
        .tmpl(data.Items).appendTo('#role-data-table tbody');

    $('#pagination').empty();
    $('#paginationTmpl')
        .tmpl(data).appendTo('#pagination');
}

function nTimesArray(n) {
    var arr = [];
    for (var i = 01; i <= n; i++) {
        arr.push(i);
    }
    return arr;
}

function createRole() {

    swal({
        title: "Create new role",
        text: "",
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        animation: "slide-from-top",
        inputPlaceholder: "Role",
        inputValue: "",
        showLoaderOnConfirm: true
    },
    function (inputValue) {
        if (inputValue === false) return false;

        if (inputValue === "") {
            swal.showInputError("Name can not be empty!");
            return false
        }
        var myurl = "/Role/Create/";
        var mydata = new Object();
        mydata = {
            'Name': inputValue
        }

        loader('show');
        $.ajax({
            type: "POST",
            url: myurl,
            data: mydata,
            dataType: "json",
            success: function (result) {
                loader('hide');
                if (result.IsSuccess) {
                    furnishDatatable(result.data);
                    swal({
                        title: "Role created successfully.",
                        text: "",
                        timer: 3000,
                        type: "success"
                    });
                }
                else {
                    swal("Failed!", result.msg, "error");
                    //showNotification(result.msg, "error");
                }

            },
            failure: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while creating role!", "error");
            },
            error: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while creating role!", "error");
            }
        });

    });

}

function editRole(element) {
    var id = $(element).attr('data-id');
    var text = $(element).attr('data-text');

    swal({
        title: "Edit role",
        text: "",
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        animation: "slide-from-top",
        inputPlaceholder: "Role",
        inputValue: text,
        showLoaderOnConfirm: true
    },
    function (inputValue) {
        if (inputValue === false) return false;

        if (inputValue === "") {
            swal.showInputError("Name can not be empty!");
            return false
        }
        var myurl = "/Role/Edit/";
        var _data = new Object();
        _data = {
            'RoleId': id,
            'Name': inputValue
        }

        loader('show');
        $.ajax({
            type: "POST",
            url: myurl,
            data: _data,
            dataType: "json",
            success: function (result) {
                loader('hide');
                if (result.IsSuccess) {
                    furnishDatatable(result.data);
                    swal({
                        title: "Role updated successfully.",
                        text: "",
                        timer: 3000,
                        type: "success"
                    });
                }
                else {
                    swal("Failed!", result.msg, "error");
                    //showNotification(result.msg, "error");
                }

            },
            failure: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while updating role!", "error");
            },
            error: function (response) {
                loader('hide');
                swal("Failed!", "Error occured while updating role!", "error");
            }
        });

    });

}

function deleteRole(element) {
    var id = $(element).attr('data-id');
    var text = $(element).attr('data-text');
    swal({
        title: "Are you sure?",
        text: "Role will be deleted and users will be removed from this role.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        closeOnConfirm: false
    },
    function () {
        var _data = {
            'RoleId': id,
            'Name': text
        };
        $.ajax({
            type: "POST",
            url: "/Role/Delete",
            data: _data,
            dataType: "json",
            success: function (response) {
                if (response.IsSuccess) {
                    furnishDatatable(response.data);
                    swal("Deleted!", "Role has been deleted.", "success");
                }
                else {
                    swal("Error!", response.msg, "error");
                }
            },
            failure: function (response) {
                //swal("Deleted!", "User has been deleted.", "error");
            },
            error: function (response) {
                //swal("Deleted!", "User has been deleted.", "error");
            }
        });
    });
}