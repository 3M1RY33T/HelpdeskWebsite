$(() => { // validationexample.js

    document.addEventListener("keyup", e => {
        $("#modalstatus").removeClass(); //remove any existing css on div
        if ($("#EmployeeModalForm").valid()) {
            $("#modalstatus").attr("class", "badge bg-primary"); //green
            $("#modalstatus").text("data entered is valid");
        }
        else {
            $("#modalstatus").attr("class", "badge bg-danger"); //red
            $("#modalstatus").text("fix errors");
        }
    });

    $("#EmployeeModalForm").validate({
        rules: {
            TextBoxTitle: { maxlength: 4, required: true, validTitle: true },
            TextBoxFirstname: { maxlength: 25, required: true },
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

    $("#getbutton").on('click', async (e) => { // event handler makes asynchronous fetch to server
        try {
            $("#TextBoxFirstname").val("");
            $("#TextBoxLastname").val("");
            $("#TextBoxEmail").val("");
            $("#TextBoxTitle").val("");
            $("#TextBoxPhone").val("");
            let validator = $("#EmployeeModalForm").validate();
            validator.resetForm();
            $("#modalstatus").attr("class", "");
            let lastname = $("#TextBoxFindLastname").val();
            $("#myModal").modal("toggle"); //pop the moda
            $("#modalstatus").text("please wait...");
            let response = await fetch(`/api/employee/${lastname}`);
            if (!response.ok) //or check for response.status
                throw new Error(`Status = ${response.status}, Text - ${response.statusText}`);
            let data = await response.json(); //this returns a promise, so we await it
            if (data.lastname !== "not found") {
                $("#TextBoxTitle").val(data.title);
                $("#TextBoxFirstname").val(data.firstname);
                $("#TextBoxLastname").val(data.lastname);
                $("#TextBoxPhone").val(data.phoneno);
                $("#TextBoxEmail").val(data.email);
                $("#modalstatus").text("employee found");
                sessionStorage.setItem("employee", JSON.stringify(data));
            } else {
                $("#TextBoxTitle").val("not found");
                $("#TextBoxFirstname").val("");
                $("#TextBoxLastname").val("");
                $("#TextBoxPhone").val("");
                $("#TextBoxEmail").val("");
                $("#modalstatus").text("no such employee");
            }
        } catch (error) {
            $("#status").text(error.message);
        } //try/catch
    }); // getbutton click event

    $('#actionbutton').click(function (e) {
        update(e);
    });

    const update = async (e) => {
        // action button click event handler
        try {
            // set up a new client side instance of Employee
            let emp = JSON.parse(sessionStorage.getItem("employee"));
            // pouplate the properties
            emp.phoneno = $("#TextBoxPhone").val();
            emp.firstname = $("#TextBoxFirstname").val();
            emp.title = $("#TextBoxTitle").val();
            emp.lastname = $("#TextBoxLastname").val();
            emp.email = $("#TextBoxEmail").val();

            console.log(emp);

            // send the updated back to the server asynchronously using Http PUT
            let response = await fetch("/api/employee", {
                method: "PUT",
                headers: { "Content-Type": "application/json; charset=utf-8" },
                body: JSON.stringify(emp),
            });
            if (response.ok) {
                // or check for response.status
                let payload = await response.json();
                $("#status").text(payload.msg);
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
    } // Update

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
    };

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
                $("#status").text("Employee Deleted"); // Set the status message to "Employee Deleted"
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
}); //main jQuery function

