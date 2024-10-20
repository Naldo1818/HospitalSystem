document.getElementById("addRowBtn").addEventListener("click", function () {
    // Create a new row
    const newRow = document.createElement("tr");

    // Create cells for task name and description
    const taskNameCell = document.createElement("td");
    const taskDescriptionCell = document.createElement("td");

    // Create input fields for the new row
    const taskNameInput = document.createElement("input");
    const taskDescriptionInput = document.createElement("input");

    // Set input attributes
    taskNameInput.setAttribute("type", "text");
    taskDescriptionInput.setAttribute("type", "text");

    // Append inputs to cells
    taskNameCell.appendChild(taskNameInput);
    taskDescriptionCell.appendChild(taskDescriptionInput);

    // Create a delete button cell
    const actionCell = document.createElement("td");
    const deleteButton = document.createElement("button");

    deleteButton.textContent = "Delete";

    // Add event listener to delete button
    deleteButton.addEventListener("click", function () {
        newRow.remove(); // Remove the row when the button is clicked
    });

    actionCell.appendChild(deleteButton);

    // Append all cells to the new row
    newRow.appendChild(taskNameCell);
    newRow.appendChild(taskDescriptionCell);
    newRow.appendChild(actionCell);

    // Append the new row to the table body
    document.querySelector("#dynamicTable tbody").appendChild(newRow);
});