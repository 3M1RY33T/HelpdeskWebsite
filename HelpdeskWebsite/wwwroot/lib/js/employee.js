$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    $("#employeeList").on('click', (e) => {
        if (!e) e = window.event;
        let id = e.target.parentNode.id;
        if (id === "employeeList" || id === "") {
            id = e.target.id;
        } // clicked on row somewhere else
        if (id !== "status" && id !== "heading") {
            let data = JSON.parse(sessionStorage.getItem("allemployees"));
            //data.forEach(employee => {
            //    if (employee.id === parseInt(id)) {
            //        $("#TextBoxPhone").val(employee.phoneno);
            //        $("#TextBoxName").val(employee.firstname);
            //        $("#TextBoxTitle").val(employee.title);
            //        $("#TextBoxLastname").val(employee.lastname);
            //        $("#TextBoxEmail").val(employee.email);
            //        sessionStorage.setItem("employee", JSON.stringify(employee));
            //        $("#modalstatus").text("update data");
            //        $("#theModal").modal("toggle");
            //    } // if
            //}); // data.map
            id === "0" ? setupForAdd() : setupForUpdate(id, data);
        } else {
            return false; // ignore if they clicked on heading or status
        }
    }); // employeeListClick


    const getAll = async (msg) => {
        try {
            $("#employeeList").text("Finding Employee Information...");
            let response = await fetch(`/api/employee`);
            if (response.ok) {
                let payload = await response.json(); // this returns a promise, so we await it
                buildEmployeeList(payload);
                // are we appending to an existing message
                msg === "" ? $("#status").text("Employees Loaded") : $("#status").text(`${msg} - Employees Loaded`);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        }
    }; // getAll

    const buildEmployeeList = (data, usealldata = true) => {
        $("#employeeList").empty("");
        div = $(`<div class="list-group-item text-white row d-flex bg-primary" id="status">Employee Info</div>
 <div class= "list-group-item row d-flex text-center" id="heading">
 <div class="col-4 h4 text-primary">Title</div>
 <div class="col-4 h4 text-primary">First</div>
 <div class="col-4 h4 text-primary">Last</div>
 </div>`);
        div.appendTo($("#employeeList"));
        usealldata ? sessionStorage.setItem("allemployees", JSON.stringify(data)) : null;
        btn = $(`<button class="list-group-item row d-flex" id="0">...click to add employee</button>`);
        btn.appendTo($("#employeeList"));
        data.forEach(emp => {
            btn = $(`<button class="list-group-item row d-flex" id="${emp.id}">`);
            btn.html(`<div class="col-4" id="employeetitle${emp.id}">${emp.title}</div>
 <div class="col-4" id="employeefname${emp.id}">${emp.firstname}</div>
 <div class="col-4" id="employeelastnam${emp.id}">${emp.lastname}</div>`
            );
            btn.appendTo($("#employeeList"));
        }); // forEach
    }; // buildEmployeeList

    getAll(""); // first grab the data from the server

    const clearModalFields = () => {
        $("#TextBoxTitle").val("");
        $("#TextBoxName").val("");
        $("#TextBoxLastname").val("");
        $("#TextBoxEmail").val("");
        $("#TextBoxPhone").val("");
        sessionStorage.removeItem("employee");
        sessionStorage.removeItem("picture");
        $("#uploadstatus").text("");
        $("#imageHolder").html("");
        $("#uploader").val("");
        $("#theModal").modal("toggle");
    }; // clearModalFields

    const setupForAdd = () => {
        $("#actionbutton").val("add");
        $("#modaltitle").html("<h4>Add Employee</h4>");
        $("#theModal").modal("toggle");
        $("#modalstatus").text("add new employee");
        $("#theModalLabel").text("Add");
        clearModalFields();
    }; // setupForAdd

    const setupForUpdate = (id, data) => {
        $("#actionbutton").val("update");
        $("#modaltitle").html("<h4>Update Employee</h4>");
        clearModalFields();
        data.forEach(employee => {
            if (employee.id === parseInt(id)) {
                $("#TextBoxTitle").val(employee.title);
                $("#TextBoxName").val(employee.firstname);
                $("#TextBoxLastname").val(employee.lastname);
                $("#TextBoxEmail").val(employee.email);
                $("#TextBoxPhone").val(employee.phoneno);
                $("#imageHolder").html(`<img height="120" width="110" src="data:img/png;base64,${employee.staffPicture64}" />`);
                // populate the other four text boxes here
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#modalstatus").text("update data");
                $("#theModal").modal("toggle");
                $("#theModalLabel").text("Update");
            } // if
        }); // data.forEach
    }; // setupForUpdate

    const add = async () => {
        try {
            emp = new Object();
            emp.title = $("#TextBoxTitle").val();
            // populate the other four properties here from the text box contents
            emp.departmentId = 100; // hard code it for now, we"ll add a dropdown later
            emp.firstname = $('#TextBoxName').val();
            emp.lastname = $('#TextBoxLastname').val();
            emp.email = $('#TextBoxEmail').val();
            emp.phoneno = $('#TextBoxPhone').val();

            emp.id = -1;
            emp.timer = null;
            emp.staffPicture64 = null;
            // send the employee info to the server asynchronously using POST
            let response = await fetch("/api/employee", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8"
                },
                body: JSON.stringify(emp)
            });
            if (response.ok) // or check for response.status
            {
                let data = await response.json();
                getAll(data.msg);
            } else if (response.status !== 404) { // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else { // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
        } // try/catch
        $("#theModal").modal("toggle");
    }; // add

    $("#actionbutton").on("click", () => {
        $("#actionbutton").val() === "update" ? update() : add();
    }); // actionbutton click

    const update = async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of Employee
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            // pouplate the properties
            emp.phoneno = $("#TextBoxPhone").val();
            emp.firstname = $("#TextBoxName").val();
            emp.title = $("#TextBoxTitle").val();
            emp.lastname = $("#TextBoxLastname").val();
            emp.email = $("#TextBoxEmail").val();
            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("/api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                getAll(payload.msg);
                $("#theModal").modal("toggle");
            } else if (response.status !== 404) {
                // probably some other client side error
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                // else 404 not found
                $("#status").text("no such path on server");
            } // else
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        } // try/catch
    }; // update

    $("#deletebutton").on('click', async (e) => {
        try {
            let emp = JSON.parse(sessionStorage.getItem("employee"));

            let response = await fetch(`/api/employee/${emp.id}`, {
                method: "DELETE",
                headers: { "Content-Type": "application/json; charset=utf-8" },
            });
            if (response.ok) {
                let payload = await response.json();
                getAll(payload.msg);
                
            } else if (response.status !== 404) {
                let problemJson = await response.json();
                errorRtn(problemJson, response.status);
            } else {
                $("#status").text("no such path on server");
            }
        } catch (error) {
            $("#status").text(error.message);
            console.table(error);
        }
    });

    //document.addEventListener("keyup", e => {
    //    $("#modalstatus").removeClass(); //remove any existing css on div
    //    if ($("#EmployeeModalForm").valid()) {
    //        $("#modalstatus").attr("class", "badge bg-primary"); //green
    //        $("#modalstatus").text("data entered is valid");
    //    }
    //    else {
    //        $("#modalstatus").attr("class", "badge bg-danger"); //red
    //        $("#modalstatus").text("fix errors");
    //    }
    //});

    $('#EmployeeModalForm').on('shown.bs.modal', function (e) {
        checkValidation();
    });

    $("#EmployeeModalForm input, #EmployeeModalForm select, #EmployeeModalForm textarea").on('keyup focusout change', function () {
        checkValidation();
    });

    const checkValidation = () => {
        if ($("#EmployeeModalForm").valid()) {
            $("#actionbutton").prop('disabled', false);
        } else {
            $("#actionbutton").prop('disabled', true);
        }
    };

    $("#EmployeeModalForm").validate({
        rules: {
            TextBoxTitle: { maxlength: 4, required: true, validTitle: true },
            TextBoxName: { maxlength: 25, required: true },
            TextBoxLastname: { maxlength: 25, required: true },
            TextBoxEmail: { maxlength: 40, required: true, email: true },
            TextBoxPhone: { maxlength: 15, required: true }
        },
        errorElement: "div",
        messages: {
            TextBoxTitle: {
                required: "required 1-4 chars.", maxlength: "required 1-4 chars.", validTitle: "Mr. Ms. Mrs. or Dr."
            },
            TextBoxFirstname: {
                required: "required 1-25 chars.", maxlength: "required 1-25 chars."
            },
            TextBoxLastname: {
                required: "required 1-25 chars.", maxlength: "required 1-25 chars."
            },
            TextBoxPhone: {
                required: "required 1-15 chars.", maxlength: "required 1-15 chars."
            },
            TextBoxEmail: {
                required: "required 1-40 chars.", maxlength: "required 1-40 chars.", email: "need valid email format"
            }
        }
    }); //EmployeeModalForm.validate

    $.validator.addMethod("validTitle", (value) => { //custome rule
        return (value === "Mr." || value === "Ms." || value === "Mrs." || value === "Dr.");
    }, ""); //.validator.addMethod

    const errorRtn = (problemJson, status) => {
        if (status > 499) {
            $("#status").text("Problem server side, see debug console");
        } else {
            let keys = Object.keys(problemJson.errors)
            problem = {
                status: status,
                statusText: problemJson.errors[keys[0]][0], // first error
            };
            $("#status").text("Problem client side, see browser console");
            console.log(problem);
        } // else
    }

    $("#srch").on("keyup", () => {
        let alldata = JSON.parse(sessionStorage.getItem("allemployees"));
        let filtereddata = alldata.filter((emp) => emp.lastname.match(new RegExp($("#srch").val(), 'i')));
        buildEmployeeList(filtereddata, false);
    }); // srch keyup

    getAll();

    $("input:file").on("change", () => {
        try {
            const reader = new FileReader();
            const file = $("#uploader")[0].files[0];
            $("#uploadstatus").text("");
            file ? reader.readAsBinaryString(file) : null;
            reader.onload = (readerEvt) => {
                // get binary data then convert to encoded string
                const binaryString = reader.result;
                const encodedString = btoa(binaryString);
                // replace the picture in session storage
                let employee = JSON.parse(sessionStorage.getItem("employee"));
                employee.staffPicture64 = encodedString;
                sessionStorage.setItem("employee", JSON.stringify(employee));
                $("#uploadstatus").text("retrieved local pic")
            };
        } catch (error) {
            $("#uploadstatus").text("pic upload failed")
        }
    }); // input file change

}); // jQuery ready method

