const API = "http://localhost:5000/api";

async function loadEvents() {
    const res = await fetch(`${API}/event`);
    const data = await res.json();

    const container = document.getElementById("events");
    container.innerHTML = "";

    data.forEach(e => {
        const btn = document.createElement("button");
        btn.innerText = e.name;

        btn.onclick = () => loadSeats(e.id);

        container.appendChild(btn);
    });
}

async function loadSeats(eventId) {
    const res = await fetch(`${API}/seats/${eventId}`);
    const seats = await res.json();

    const container = document.getElementById("seats");
    container.innerHTML = "";

    seats.forEach(seat => {
        const div = document.createElement("div");

        div.className = `seat ${seat.status.toLowerCase()}`;
        div.innerText = seat.number;

        if (seat.status === "Disponible") {
            div.onclick = () => reserveSeat(seat.id);
        }

        container.appendChild(div);
    });
}

async function reserveSeat(seatId) {
    try {
        const res = await fetch(`${API}/seats/reserve/${seatId}`, {
            method: "POST"
        });

        if (res.status === 409) {
            alert("Otro usuario tomó este asiento 😢");
        } else {
            alert("Reservado!");
        }

        loadEvents();
    } catch {
        alert("Error");
    }
}

// 🔥 AUDITORÍA
async function loadAudit() {
    const res = await fetch(`${API}/audit`);
    const logs = await res.json();

    const container = document.getElementById("audit");
    container.innerHTML = "";

    logs.forEach(log => {
        const div = document.createElement("div");

        div.innerText = `
        ${log.timestamp} - ${log.action} - Seat ${log.seatId} - ${log.user}
        `;

        container.appendChild(div);
    });
}

loadEvents();