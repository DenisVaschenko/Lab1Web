const API_BASE_URL = "https://localhost:7260/api/";
let currPage = 1;
const pageSize = 10;
document.getElementById('studentsBtn').addEventListener('click',  () => {
     fetchEntity("student")});
document.getElementById('coursesBtn').addEventListener('click',  () => {
    fetchEntity("course")});
document.getElementById('instructorsBtn').addEventListener('click',  () => {
    fetchEntity("instructor")});

function GetRequestBody(entity){
    if (entity == "student"){
        return {
            "name": "string",
            "age": 18,
            "email": "email@gmail.com",
            "phone": "+00000000000",
            "group": "kp-23",
            "averageScore": 75
        }
    }
    if (entity == "instructor"){
        return {
            "name": "string",
            "age": 0,
            "email": "email@gmail.com",
            "phone": "+00000000000",
            "specialisation": "string",
            "degree": "string"
        }
    }
    if (entity ){
        return {
            "title": "string",
            "duration": 20,
            "difficulty": "string"
        }
    }
    return null;
}
async function fetchEntity(name) {
    console.log(`${API_BASE_URL}${name}?page=${currPage}&pageSize=${pageSize}`);
    try {
        const tableContainer = document.getElementById("tableContainer");
        let response = await fetch(`${API_BASE_URL}${name}?page=${currPage}&pageSize=${pageSize}`);
        let data = await response.json();
        if (data && data.length > 0){
            createTable(data);
            const buttonDeleteAll = document.createElement("button");
            buttonDeleteAll.innerText = "Delete All";
            buttonDeleteAll.addEventListener('click', () => fetchDeleteAll(name));
            tableContainer.appendChild(buttonDeleteAll);
            createNavigation(name);
            createFindRequest(name);
            createPostRequest(name);
        } else{
            console.error("data is not found")
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
function createTable(data) {
    const tableContainer = document.getElementById("tableContainer");

    tableContainer.innerHTML = "";

    const table = document.createElement("table");
    table.style.border = "1px solid black";
    table.style.borderCollapse = "collapse";

    const header = table.createTHead();
    const headerRow = header.insertRow();
    Object.keys(data[0]).forEach(key => {
        if (Array.isArray(data[0][key])) return null;
        const th = document.createElement("th");
        th.innerText = key.charAt(0).toUpperCase() + key.slice(1);
        th.style.border = "1px solid black";
        th.style.padding = "5px";
        headerRow.appendChild(th);
    });

    // Створення тіла таблиці
    const tbody = table.createTBody();
    data.forEach(item => {
        const row = tbody.insertRow();
        Object.values(item).forEach(value => {
            if (Array.isArray(value)) return null;
            const td = document.createElement("td");
            td.innerText = value;
            td.style.border = "1px solid black";
            td.style.padding = "5px";
            row.appendChild(td);
        });
    });

    tableContainer.appendChild(table);
}
function createNavigation(name){
    const buttonPrev = document.createElement("button");
    buttonPrev.innerText = "previous page";
    buttonPrev .addEventListener('click',  () => {
        currPage = Math.max(1, currPage - 1);
        fetchEntity(name);
    });
    const buttonNext = document.createElement("button");
    buttonNext.innerText="next page";
    buttonNext.addEventListener('click',  () => {
        currPage = currPage+1;
        fetchEntity(name);
    });
    const tableContainer = document.getElementById("tableContainer");
    tableContainer.appendChild(buttonPrev);
    tableContainer.appendChild(buttonNext);
    const paragraph = document.createElement("p");
        paragraph.textContent = `Current page: ${currPage}`;
    tableContainer.appendChild(paragraph);
}
function createFindRequest(name){
    const paragraph = document.createElement("p");
    const inputField = document.createElement("input");
    inputField.type = "number";
    paragraph.appendChild(inputField);
    const buttonFind = document.createElement("button");
    buttonFind.innerText = "Знайти";
    paragraph.appendChild(buttonFind);
    buttonFind.addEventListener('click', () => { fetchEntityFind(name, inputField.value)})
    const tableContainer = document.getElementById("tableContainer");
    tableContainer.appendChild(paragraph);
}
async function fetchEntityFind(name, id){
    console.log(`${API_BASE_URL}${name}/${id}`);
    try {
        let response = await fetch(`${API_BASE_URL}${name}/${id}`);
        let data = await response.json();
        const tableContainer = document.getElementById("tableContainer");
        tableContainer.innerHTML = "";
        for (const [key,value] of Object.entries(data)){
            if (!Array.isArray(value)) {
                const p = document.createElement("p");
                p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
                tableContainer.appendChild(p);
            } else{
                let buttonAttach = document.createElement("button");
                buttonAttach.innerText = `attach ${key}`;
                let inputAttach =document.createElement("input");
                let inputDetach =document.createElement("input");
                let buttonDetach = document.createElement("button");
                buttonDetach.innerText = `detach ${key}`;
                buttonAttach.addEventListener('click', () => {
                    const message =JSON.stringify(inputAttach.value.split(',').map(item => {
                        const number = parseFloat(item.trim());
                        return number
                    }));
                    fetchEntityAttach(name, id, key, message);
                    console.log(message);
                });
                buttonDetach.addEventListener('click', () => {
                    const message =JSON.stringify(inputDetach.value.split(',').map(item => {
                        const number = parseFloat(item.trim());
                        return number
                    }));
                    fetchEntityDetach(name, id, key, message);
                    console.log(message);
                });
                tableContainer.appendChild(inputAttach);
                tableContainer.appendChild(buttonAttach);
                tableContainer.appendChild(document.createElement("br"));
                tableContainer.appendChild(inputDetach);
                tableContainer.appendChild(buttonDetach);
            }

        }
        const buttonDelete = document.createElement("button");
        buttonDelete.innerText = "Delete";
        buttonDelete.addEventListener('click', () => {
            fetchDelete(name, id);
            fetchEntity(name);
        });
        tableContainer.appendChild(document.createElement("br"));
        tableContainer.appendChild(buttonDelete);
        const buttonUpdate = document.createElement("button");
        buttonUpdate.innerText = "Update";
        buttonUpdate.addEventListener('click', () => {
            createChangeRequest(name, id);
        });
        tableContainer.appendChild(buttonUpdate);
    } catch (error) {
        console.error('Error:', error);
    }
}

async function fetchEntityAttach(name, id, attaching, attachingsId){
    console.log(`${API_BASE_URL}${name}/attach${attaching}/${id}`);
    try {
        let response = await fetch(`${API_BASE_URL}${name}/attach${attaching}/${id}`, {
            method: 'put',
            headers: {
                'Content-Type': 'application/json'
            },
            body: attachingsId
        });
        data = await response.json();
        const tableContainer = document.getElementById("tableContainer");
        tableContainer.innerHTML = "";
        for (const [key,value] of Object.entries(data)){
            if (!Array.isArray(value)){
            const p = document.createElement("p");
            p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
            tableContainer.appendChild(p);
            } else if (key == attaching){
                const p = document.createElement("b");
                p.innerText = `${key[0].toUpperCase() + key.slice(1)}:`;
                tableContainer.appendChild(p);
                value.forEach(item => {
                    for (const [key,value] of Object.entries(item)){
                        const p = document.createElement("p");
                        p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
                        tableContainer.appendChild(p);
                    } 
                })
                tableContainer.appendChild(document.createElement("br"));
            }
        }
        const p = document.createElement("p");
        p.innerText = data;
    } catch (error) {
        console.error('Error:', error);
    }
}
async function fetchEntityDetach(name, id, detaching, detachingsId){
    console.log(`${API_BASE_URL}${name}/detach${detaching}/${id}`);
    try {
        let response = await fetch(`${API_BASE_URL}${name}/detach${detaching}/${id}`, {
            method: 'put',
            headers: {
                'Content-Type': 'application/json'
            },
            body: detachingsId
        });
        data = await response.json();
        const tableContainer = document.getElementById("tableContainer");
        tableContainer.innerHTML = "";
        for (const [key,value] of Object.entries(data)){
            if (!Array.isArray(value)){
            const p = document.createElement("p");
            p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
            tableContainer.appendChild(p);
            } else if (key == detaching){
                const p = document.createElement("b");
                p.innerText = `${key[0].toUpperCase() + key.slice(1)}:`;
                tableContainer.appendChild(p);
                value.forEach(item => {
                    for (const [key,value] of Object.entries(item)){
                        const p = document.createElement("p");
                        p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
                        tableContainer.appendChild(p);
                    } 
                })
                tableContainer.appendChild(document.createElement("br"));
            }
        }
        const p = document.createElement("p");
        p.innerText = data;
    } catch (error) {
        console.error('Error:', error);
    }
}
async function  fetchEntityPost(name, body) {
    console.log(`${API_BASE_URL}${name}/`);
    try{
        let response = await fetch(`${API_BASE_URL}${name}`, {
            method: 'post',
            headers: {
                'Content-Type': 'application/json'
            },
            body: body
        });
        data = await response.json();
        const tableContainer = document.getElementById("tableContainer");
        tableContainer.innerHTML = "";
        for (const [key,value] of Object.entries(data)){
            if (!Array.isArray(value)){
            const p = document.createElement("p");
            p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
            tableContainer.appendChild(p);
            }
        }
    }catch (error) {
        console.error('Error:', error);
    }
}
function createPostRequest(name){
    const tableContainer = document.getElementById("tableContainer");
    const newBody = getInputEntity(name);
    const button = document.createElement("button");
    button.innerText = `Create ${name}`;
    button.addEventListener('click', () => {
        for (const [key,value] of Object.entries(newBody)){
            if (!isNaN(parseFloat(value.value.trim()))){newBody[key]=parseFloat(value.value.trim());}
            else {newBody[key]=value.value;}
        };
        console.log(JSON.stringify(newBody));
        fetchEntityPost(name,JSON.stringify(newBody));
    });
    tableContainer.appendChild(button);
}
async function fetchDelete(name, id){
    console.log(`${API_BASE_URL}${name}/${id}`);
    try{
        let response = await fetch(`${API_BASE_URL}${name}/${id}`, {
            method: 'delete'
        });
    }
    catch (error) {
        console.error('Error:', error);
    }
}
async function fetchDeleteAll(name){
    console.log(`${API_BASE_URL}${name}`);
    try{
        let response = await fetch(`${API_BASE_URL}${name}`, {
            method: 'delete'
        });
    }
    catch (error) {
        console.error('Error:', error);
    }
}
function getInputEntity(name){
    const body = GetRequestBody(name);
    const newBody = {};
    const tableContainer = document.getElementById("tableContainer");
    for (const [key,value] of Object.entries(body)){
        const textNode = document.createTextNode(`${key}: `)
        const input = document.createElement("input");
        input.value = value;
        newBody[key]=input;
        tableContainer.appendChild(textNode);
        tableContainer.appendChild(input);
        tableContainer.appendChild(document.createElement("br"));
    };
    return newBody;
}
function createChangeRequest(name, id){
    const tableContainer = document.getElementById("tableContainer");
    tableContainer.innerHTML = "";
    const newBody = getInputEntity(name);
    const button = document.createElement("button");
    button.innerText = `CUpdate ${name} with id ${id}`;
    button.addEventListener('click', () => {
        for (const [key,value] of Object.entries(newBody)){
            if (!isNaN(parseFloat(value.value.trim()))){newBody[key]=parseFloat(value.value.trim());}
            else {newBody[key]=value.value;}
        };
        console.log(JSON.stringify(newBody));
        fetchChange(name, id, JSON.stringify(newBody));
    });
    tableContainer.appendChild(button);
}
async function fetchChange(name, id, body){
    console.log(`${API_BASE_URL}${name}/${id}`);
    try{
        let response = await fetch(`${API_BASE_URL}${name}/${id}`, {
            method: 'put',
            headers: {
                'Content-Type': 'application/json'
            },
            body: body
        });
        console.log(body);
        data = await response.json();
        const tableContainer = document.getElementById("tableContainer");
        tableContainer.innerHTML = "";
        for (const [key,value] of Object.entries(data)){
            if (!Array.isArray(value)){
            const p = document.createElement("p");
            p.innerText = `${key[0].toUpperCase() + key.slice(1)}: ${value}`;
            tableContainer.appendChild(p);
            }
        }
    }
    catch (error) {
        console.error('Error:', error);
    }
}
function removeTable() {
    const tableContainer = document.getElementById("tableContainer");
    tableContainer.innerHTML = "";
}