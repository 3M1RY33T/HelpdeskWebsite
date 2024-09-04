$(() => { // main jQuery routine - executes every on page load, $ is short for jquery
    const buildDynamicList = () => {
        $("#dynamicList").empty();
        div = $(`<div class="list-group-item text-white bg-secondary row d-flex" id="status">Sample Bootstrap Styling</div>
 <div class= "list-group-item row d-flex text-center" id="heading">
 <div class="col-4 h4">Column 1</div>
<div class="col-4 h4">Column 2</div>
<div class="col-4 h4">Column 3</div>
 </div>`);
        div.appendTo($("#dynamicList"));
        let data = [
            { "col1": "Some", "col2": "Employee", "col3": "Data" },
            { "col1": "Will", "col2": "Be", "col3": "Listed" },
            { "col1": "In", "col2": "Thes", "col3": "Columns" },
            { "col1": "Try", "col2": "Clicking", "col3": "One Row" }
        ];
        data.forEach(row => {
            btn = $(`<button class="list-group-item row d-flex" onclick="confirm('this will eventually be a modal');">`);
            btn.html(`<div class="col-4">${row.col1}</div>
 <div class="col-4">${row.col2}</div>
 <div class="col-4">${row.col3}</div>`
            );
            btn.appendTo($("#dynamicList"));
        }); // map
    }; // buildDynamicList
    buildDynamicList();
}); // jQuery ready method