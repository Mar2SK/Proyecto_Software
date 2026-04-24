const API_URL = "http://localhost:5123/api/events";

async function loadEvents() {
    const res = await fetch(API_URL);
    const events = await res.json();

    const container = document.getElementById("eventsContainer");
    container.innerHTML = "";

    events.forEach(e => {
        const card = document.createElement("div");
        card.className = "card";

        card.innerHTML = `
            <img src="https://picsum.photos/400/200?random=${e.id}">
            <div class="card-content">
                <h3>${e.name}</h3>
                <p>${e.venue}</p>
                <button class="btn" onclick="goToSeats(${e.id})">
                    Comprar
                </button>
            </div>
        `;

        container.appendChild(card);
    });
}

function goToSeats(eventId) {
    window.location.href = `seats.html?eventId=${eventId}`;
}

loadEvents();