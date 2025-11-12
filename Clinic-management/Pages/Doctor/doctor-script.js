// doctor-script.js
const API_BASE = '/api/doctor';
let currentUser = JSON.parse(sessionStorage.getItem('currentUser') || '{}');
let currentAppointment = null;

// === DASHBOARD ===
async function loadDoctorDashboard() {
    if (!currentUser.staff_id) return alert('Vui lòng đăng nhập');

    document.getElementById('doctorName').textContent = currentUser.fullname;
    document.getElementById('specialization').textContent = currentUser.specialization || 'Chưa xác định';

    try {
        const [apptRes, shiftRes, recordRes, stockRes] = await Promise.all([
            fetch(`${API_BASE}/appointments/today?doctorId=${currentUser.staff_id}`),
            fetch(`${API_BASE}/shift/today?staffId=${currentUser.staff_id}`),
            fetch(`${API_BASE}/records/today?doctorId=${currentUser.staff_id}`),
            fetch(`${API_BASE}/inventory/low`)
        ]);

        const appointments = await apptRes.json();
        const shift = await shiftRes.json();
        const records = await recordRes.json();
        const lowStock = await stockRes.json();

        document.getElementById('todayAppointments').textContent = appointments.length;
        document.getElementById('waitingPatients').textContent = appointments.filter(a => a.status === 'booked').length;
        document.getElementById('todayRecords').textContent = records.length;
        document.getElementById('lowStockItems').textContent = lowStock.length;
        document.getElementById('todayShift').textContent = shift ? `${shift.start_time} - ${shift.end_time}` : 'Không có';

        renderAppointmentsTable(appointments);
    } catch (err) { console.error(err); }
}

function renderAppointmentsTable(data) {
    const tbody = document.querySelector('#appointmentsTable tbody');
    tbody.innerHTML = data.map(a => `
        <tr>
            <td>${new Date(a.appointment_date).toLocaleTimeString('vi')}</td>
            <td>${a.patient_name}</td>
            <td>${a.service_name}</td>
            <td><span class="status-${a.status}">${a.status === 'booked' ? 'Đang chờ' : 'Hoàn thành'}</span></td>
            <td><button onclick="startExam(${a.appointment_id})" class="btn-small">Khám</button></td>
        </tr>
    `).join('');
}

function startExam(id) {
    window.location.href = `/Pages/Doctor/DoctorExamination.cshtml?id=${id}`;
}

function openExamination() {
    const first = document.querySelector('#appointmentsTable tbody tr');
    if (first) startExam(first.dataset.id);
    else alert('Không có lịch hẹn nào');
}

function viewShift() {
    alert('Tính năng xem ca trực tuần đang phát triển');
}

// === EXAMINATION ===
async function loadExamination(apptId) {
    const res = await fetch(`${API_BASE}/appointment/${apptId}`);
    currentAppointment = await res.json();

    document.getElementById('patientName').textContent = currentAppointment.patient_name;
    document.getElementById('patientPhone').textContent = currentAppointment.phone;
    document.getElementById('patientDOB').textContent = new Date(currentAppointment.date_of_birth).toLocaleDateString('vi');
    document.getElementById('patientGender').textContent = currentAppointment.gender;
    document.getElementById('patientInsurance').textContent = currentAppointment.insurance || 'Không';

    loadPatientHistory(currentAppointment.patient_id);
}

async function loadPatientHistory(patientId) {
    const res = await fetch(`${API_BASE}/patient/${patientId}/history`);
    const history = await res.json();
    const container = document.getElementById('historyList');
    container.innerHTML = history.length ? history.map(h => `
        <div class="history-item">
            <p><strong>${new Date(h.record_date).toLocaleDateString('vi')}:</strong> ${h.diagnosis}</p>
            <p><em>Điều trị:</em> ${h.treatment}</p>
        </div>
    `).join('') : '<p>Chưa có bệnh án</p>';
}

async function loadServices() {
    const res = await fetch('/api/services');
    const services = await res.json();
    const select = document.getElementById('serviceSelect');
    select.innerHTML = services.map(s => `<option value="${s.service_id}">${s.service_name} (${s.price.toLocaleString()}đ)</option>`).join('');
}

async function loadMedicines() {
    const res = await fetch('/api/medicines');
    const meds = await res.json();
    document.querySelectorAll('.medicine-select').forEach(sel => {
        sel.innerHTML = '<option value="">Chọn thuốc</option>' + meds.map(m => `<option value="${m.medicine_id}">${m.name} (${m.price.toLocaleString()}đ/${m.unit})</option>`).join('');
    });
}

document.getElementById('addMedicine')?.addEventListener('click', () => {
    const container = document.getElementById('prescriptionList');
    const row = document.createElement('div');
    row.className = 'prescription-row';
    row.innerHTML = `
        <select class="medicine-select"></select>
        <input type="text" placeholder="Liều lượng" class="dosage-input" />
        <input type="number" placeholder="SL" min="1" class="qty-input" />
        <button type="button" class="remove-med">X</button>
    `;
    container.appendChild(row);
    loadMedicines();
    row.querySelector('.remove-med').onclick = () => row.remove();
});

async function saveRecord() {
    const data = {
        appointment_id: currentAppointment.appointment_id,
        diagnosis: document.getElementById('diagnosis').value,
        treatment: document.getElementById('treatment').value,
        services: Array.from(document.getElementById('serviceSelect').selectedOptions).map(o => ({ service_id: o.value })),
        prescriptions: Array.from(document.querySelectorAll('.prescription-row')).map(r => ({
            medicine_id: r.querySelector('.medicine-select').value,
            dosage: r.querySelector('.dosage-input').value,
            quantity: r.querySelector('.qty-input').value
        })).filter(p => p.medicine_id)
    };

    await fetch(`${API_BASE}/record/save`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) });
    alert('Lưu bệnh án thành công!');
}

async function createInvoice() {
    await saveRecord();
    await fetch(`${API_BASE}/invoice/create?appointmentId=${currentAppointment.appointment_id}`, { method: 'POST' });
    alert('Hóa đơn đã được tạo!');
}

async function completeAppointment() {
    await fetch(`${API_BASE}/appointment/complete/${currentAppointment.appointment_id}`, { method: 'POST' });
    alert('Hoàn thành khám!');
    window.location.href = '/Pages/Doctor/DoctorDashboard.cshtml';
}

// === PRESCRIPTION & INVOICE ===
async function loadPrescriptions() {
    const res = await fetch(`${API_BASE}/prescriptions?doctorId=${currentUser.staff_id}`);
    const data = await res.json();
    const tbody = document.querySelector('#prescriptionTable tbody');
    tbody.innerHTML = data.map(p => `
        <tr>
            <td>${new Date(p.record_date).toLocaleDateString('vi')}</td>
            <td>${p.patient_name}</td>
            <td>${p.medicine_name}</td>
            <td>${p.dosage}</td>
            <td>${p.quantity}</td>
        </tr>
    `).join('');
}

async function loadInvoices() {
    const res = await fetch(`${API_BASE}/invoices?doctorId=${currentUser.staff_id}`);
    const data = await res.json();
    const tbody = document.querySelector('#invoiceTable tbody');
    tbody.innerHTML = data.map(i => `
        <tr>
            <td>#${i.invoice_id}</td>
            <td>${i.patient_name}</td>
            <td>${i.total_amount.toLocaleString()}đ</td>
            <td><span class="status-${i.status}">${i.status === 'paid' ? 'Đã thanh toán' : 'Chưa thanh toán'}</span></td>
            <td><button onclick="printInvoice(${i.invoice_id})" class="btn-small">In</button></td>
        </tr>
    `).join('');
}

function printInvoice(id) {
    window.open(`/Pages/Doctor/PrintInvoice.cshtml?id=${id}`, '_blank');
}