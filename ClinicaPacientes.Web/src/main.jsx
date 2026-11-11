import React, { useEffect, useMemo, useState } from 'react'
import { createRoot } from 'react-dom/client'
import './style.css'

const API = import.meta.env.VITE_API_URL || 'http://localhost:5130'

const modules = [
  { key: 'dashboard', label: 'Resumen', icon: '▦', route: '/api/Dashboard' },
  { key: 'Pacientes', label: 'Pacientes', icon: '♙', fields: [
    ['nombres', 'Nombres', 'text', true], ['apellidos', 'Apellidos', 'text', true], ['documento', 'Documento', 'text', true], ['fechaNacimiento', 'Fecha de nacimiento', 'date', true], ['sexo', 'Sexo', 'select', true, ['Femenino', 'Masculino', 'Otro']], ['telefono', 'Teléfono', 'text'], ['correo', 'Correo electrónico', 'email'], ['direccion', 'Dirección', 'text']
  ], columns: ['nombres', 'apellidos', 'documento', 'telefono', 'correo'] },
  { key: 'Medicos', label: 'Médicos', icon: '⚕', fields: [
    ['nombres', 'Nombres', 'text', true], ['apellidos', 'Apellidos', 'text', true], ['correo', 'Correo electrónico', 'email'], ['telefono', 'Teléfono', 'text'], ['especialidadId', 'Especialidad', 'ref', true, 'Especialidades']
  ], columns: ['nombres', 'apellidos', 'correo', 'telefono', 'especialidadId'] },
  { key: 'Especialidades', label: 'Especialidades', icon: '✚', fields: [['nombre', 'Nombre', 'text', true]], columns: ['nombre'] },
  { key: 'Citas', label: 'Citas', icon: '▣', fields: [
    ['pacienteId', 'Paciente', 'ref', true, 'Pacientes'], ['medicoId', 'Médico', 'ref', true, 'Medicos'], ['fechaHora', 'Fecha y hora', 'datetime-local', true], ['motivo', 'Motivo', 'text', true], ['estado', 'Estado', 'select', true, ['Programada', 'Completada', 'Cancelada']]
  ], columns: ['pacienteId', 'medicoId', 'fechaHora', 'motivo', 'estado'] },
  { key: 'HistorialesMedicos', label: 'Historial médico', icon: '▤', fields: [
    ['pacienteId', 'Paciente', 'ref', true, 'Pacientes'], ['fecha', 'Fecha', 'datetime-local', true], ['diagnostico', 'Diagnóstico', 'text', true], ['observaciones', 'Observaciones', 'textarea'], ['tratamiento', 'Tratamiento', 'textarea']
  ], columns: ['pacienteId', 'fecha', 'diagnostico', 'tratamiento'] },
  { key: 'Medicamentos', label: 'Medicamentos', icon: '✚', fields: [['nombre', 'Nombre', 'text', true], ['presentacion', 'Presentación', 'text'], ['precio', 'Precio', 'number', true]], columns: ['nombre', 'presentacion', 'precio'] },
  { key: 'Facturas', label: 'Facturas', icon: '▧', fields: [
    ['citaId', 'Cita', 'ref', true, 'Citas'], ['fecha', 'Fecha', 'datetime-local', true], ['total', 'Total', 'number', true], ['estado', 'Estado', 'select', true, ['Pendiente', 'Pagada', 'Anulada']]
  ], columns: ['citaId', 'fecha', 'total', 'estado'] },
  { key: 'DetallesFactura', label: 'Detalles de factura', icon: '☷', fields: [
    ['facturaId', 'Factura', 'ref', true, 'Facturas'], ['medicamentoId', 'Medicamento', 'ref', true, 'Medicamentos'], ['cantidad', 'Cantidad', 'number', true], ['precioUnitario', 'Precio unitario', 'number', true]
  ], columns: ['facturaId', 'medicamentoId', 'cantidad', 'precioUnitario'] },
  { key: 'SegurosMedicos', label: 'Seguros médicos', icon: '▱', fields: [
    ['pacienteId', 'Paciente', 'ref', true, 'Pacientes'], ['aseguradora', 'Aseguradora', 'text', true], ['numeroPoliza', 'Número de póliza', 'text', true], ['vigenciaHasta', 'Vigencia hasta', 'date']
  ], columns: ['pacienteId', 'aseguradora', 'numeroPoliza', 'vigenciaHasta'] },
  { key: 'Usuarios', label: 'Usuarios', icon: '♙', fields: [
    ['nombre', 'Nombre', 'text', true], ['correo', 'Correo electrónico', 'email', true], ['rol', 'Rol', 'select', true, ['Administrador', 'Recepcionista', 'Medico']], ['password', 'Contraseña', 'password'], ['activo', 'Estado', 'select', true, ['true', 'false']]
  ], columns: ['nombre', 'correo', 'rol', 'activo'] }
]

const emptyForm = fields => Object.fromEntries(fields.map(([name]) => [name, '']))
const dateValue = value => value ? String(value).slice(0, 16) : ''

function App() {
  const [active, setActive] = useState('dashboard')
  const [data, setData] = useState([])
  const [lookups, setLookups] = useState({})
  const [stats, setStats] = useState({})
  const [form, setForm] = useState({})
  const [editing, setEditing] = useState(null)
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')
  const current = modules.find(x => x.key === active)
  const isDashboard = active === 'dashboard'

  const request = async (url, options = {}) => {
    const response = await fetch(`${API}${url}`, { ...options, headers: { 'Content-Type': 'application/json', ...(options.headers || {}) } })
    if (response.status === 204) return null
    const body = await response.json().catch(() => null)
    if (!response.ok) throw new Error(body?.mensaje || body?.title || `Error ${response.status}: no se pudo completar la operación.`)
    return body
  }

  const load = async () => {
    setLoading(true)
    setError('')
    try {
      if (isDashboard) {
        const result = await request('/api/Dashboard')
        setStats(result)
      } else {
        const url = active === 'Pacientes' && search ? `/api/${active}?busqueda=${encodeURIComponent(search)}` : `/api/${active}`
        const result = await request(url)
        setData(result)
      }
    } catch (e) {
      setError(`${e.message} Comprueba que la API esté ejecutándose en ${API}.`)
    } finally {
      setLoading(false)
    }
  }

  const loadLookups = async () => {
    const needed = [...new Set((current?.fields || []).filter(f => f[2] === 'ref').map(f => f[4]))]
    if (!needed.length) return
    try {
      const values = await Promise.all(needed.map(async key => [key, await request(`/api/${key}`)]))
      setLookups(previous => ({ ...previous, ...Object.fromEntries(values) }))
    } catch (e) {
      setError(e.message)
    }
  }

  useEffect(() => { load(); setEditing(null); setForm(current?.fields ? emptyForm(current.fields) : {}) }, [active])
  useEffect(() => { loadLookups() }, [active])

  const change = (name, value) => setForm(previous => ({ ...previous, [name]: value }))

  const startEdit = row => {
    setEditing(row.id)
    const next = {}
    current.fields.forEach(([name, label, type]) => {
      let value = row[name]
      if (type === 'datetime-local') value = dateValue(value)
      else if (type === 'date') value = value ? String(value).slice(0, 10) : ''
      else if (value === true || value === false) value = String(value)
      next[name] = value ?? ''
    })
    setForm(next)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const save = async event => {
    event.preventDefault()
    setError('')
    const payload = {}
    current.fields.forEach(([name, label, type, required]) => {
      let value = form[name]
      if (name === 'password' && value === '') return
      if (value === '' && type === 'date' && !required) value = null
      else if (type === 'number' || type === 'ref') value = Number(value)
      else if ((type === 'datetime-local' || type === 'date') && value !== '') value = new Date(value).toISOString()
      if (name === 'activo') value = value === 'true'
      payload[name] = value
    })
    if (active === 'Usuarios' && !editing && !payload.password) {
      setError('La contraseña es obligatoria para crear un usuario.')
      return
    }
    try {
      await request(`/api/${active}${editing ? `/${editing}` : ''}`, { method: editing ? 'PUT' : 'POST', body: JSON.stringify(payload) })
      setNotice(editing ? 'Registro actualizado correctamente.' : 'Registro creado correctamente.')
      setEditing(null)
      setForm(emptyForm(current.fields))
      await load()
    } catch (e) { setError(e.message) }
  }

  const remove = async row => {
    if (!window.confirm('¿Seguro que deseas eliminar este registro?')) return
    try {
      await request(`/api/${active}/${row.id}`, { method: 'DELETE' })
      setNotice('Registro eliminado correctamente.')
      await load()
    } catch (e) { setError(e.message) }
  }

  const display = (row, column) => {
    const value = row[column]
    if (value === null || value === undefined || value === '') return '—'
    if (column.toLowerCase().endsWith('id')) {
      const relation = column.slice(0, -2)
      const ref = row[relation]
      if (ref) return ref.nombre || `${ref.nombres || ''} ${ref.apellidos || ''}`.trim() || `#${value}`
      const map = { paciente: 'Pacientes', medico: 'Medicos', especialidad: 'Especialidades', cita: 'Citas', factura: 'Facturas', medicamento: 'Medicamentos' }
      const found = (lookups[map[relation]] || []).find(x => x.id === value)
      return found ? (found.nombre || `${found.nombres || ''} ${found.apellidos || ''}`.trim() || `#${value}`) : `#${value}`
    }
    if (column.toLowerCase().includes('fecha')) return new Date(value).toLocaleString('es-DO')
    if (typeof value === 'boolean') return value ? 'Activo' : 'Inactivo'
    return String(value)
  }

  const filtered = useMemo(() => {
    if (!search) return data
    return data.filter(row => Object.entries(row).some(([key, value]) => ['nombres', 'apellidos', 'documento', 'nombre', 'correo', 'motivo', 'diagnostico', 'aseguradora', 'numeroPoliza'].includes(key) && typeof value === 'string' && value.toLowerCase().includes(search.toLowerCase())))
  }, [data, search])

  const renderField = field => {
    const [name, label, type, required, options] = field
    const value = form[name] ?? ''
    if (type === 'ref') return <label key={name}>{label}<select required={required} value={value} onChange={e => change(name, e.target.value)}><option value="">Seleccionar</option>{(lookups[options] || []).map(item => <option key={item.id} value={item.id}>{item.nombre || `${item.nombres || ''} ${item.apellidos || ''}`.trim() || `Registro ${item.id}`}</option>)}</select></label>
    if (type === 'select') return <label key={name}>{label}<select required={required} value={value} onChange={e => change(name, e.target.value)}><option value="">Seleccionar</option>{options.map(option => <option key={option} value={option}>{name === 'activo' ? option === 'true' ? 'Activo' : 'Inactivo' : option}</option>)}</select></label>
    return <label key={name} className={type === 'textarea' ? 'wide' : ''}>{label}{type === 'textarea' ? <textarea required={required} value={value} onChange={e => change(name, e.target.value)} rows="3"/> : <input type={type} required={required} min={type === 'number' ? '0' : undefined} step={type === 'number' ? '0.01' : undefined} value={value} onChange={e => change(name, e.target.value)} />}</label>
  }

  return <div className="app-shell">
    <aside className="sidebar">
      <div className="brand"><span className="brand-mark">✚</span><div><strong>CLÍNICA</strong><small>GESTIÓN INTEGRAL</small></div></div>
      <div className="nav-caption">MENÚ PRINCIPAL</div>
      <nav>{modules.map(item => <button key={item.key} className={active === item.key ? 'nav-item active' : 'nav-item'} onClick={() => { setActive(item.key); setSearch(''); setError(''); setNotice('') }}><span>{item.icon}</span>{item.label}</button>)}</nav>
      <div className="sidebar-bottom"><div className="online-dot"/> API REST conectada <small>ASP.NET Core · SQLite</small></div>
    </aside>
    <main className="main-area">
      <header className="topbar"><div><span className="eyebrow">SISTEMA DE ADMINISTRACIÓN</span><h1>{isDashboard ? 'Panel general' : current.label}</h1></div><div className="top-right"><span className="today">{new Date().toLocaleDateString('es-DO', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })}</span><div className="profile">AD</div></div></header>
      <div className="content">
        {error && <div className="alert error-alert">{error}<button onClick={() => setError('')}>×</button></div>}
        {notice && <div className="alert success-alert">{notice}<button onClick={() => setNotice('')}>×</button></div>}
        {isDashboard ? <>
          <section className="welcome"><div><span className="eyebrow light">BIENVENIDO AL SISTEMA</span><h2>Controla tu clínica desde un solo lugar.</h2><p>Consulta los indicadores y administra la información médica.</p></div><div className="welcome-icon">✚</div></section>
          <section className="metric-grid">
            <Metric icon="♙" label="Pacientes registrados" value={stats.pacientes ?? '—'} color="teal"/>
            <Metric icon="⚕" label="Médicos" value={stats.medicos ?? '—'} color="blue"/>
            <Metric icon="▣" label="Citas programadas" value={stats.citasPendientes ?? '—'} color="purple"/>
            <Metric icon="✚" label="Medicamentos" value={stats.medicamentos ?? '—'} color="orange"/>
          </section>
          <section className="dashboard-grid"><div className="panel"><div className="panel-heading"><div><span className="eyebrow">ACTIVIDAD</span><h3>Resumen operativo</h3></div><span className="panel-icon">▦</span></div><div className="summary-row"><span>Total de citas</span><strong>{stats.citas ?? 0}</strong></div><div className="summary-row"><span>Facturas pendientes</span><strong>{stats.facturasPendientes ?? 0}</strong></div><div className="summary-row"><span>Ingresos de facturas pagadas</span><strong>{Number(stats.ingresos || 0).toLocaleString('es-DO', { style: 'currency', currency: 'DOP' })}</strong></div></div><div className="panel quick-panel"><span className="eyebrow">ACCESOS RÁPIDOS</span><h3>Administración</h3><div className="quick-grid">{modules.filter(x => x.key !== 'dashboard').slice(0, 6).map(item => <button key={item.key} onClick={() => setActive(item.key)}><span>{item.icon}</span>{item.label}<b>→</b></button>)}</div></div></section>
        </> : <>
          <div className="section-intro"><div><span className="eyebrow">GESTIÓN DE REGISTROS</span><p>Registra, consulta, modifica y elimina información del sistema.</p></div><div className="record-total">{data.length} registros</div></div>
          <div className="workspace">
            <section className="panel editor-panel"><div className="panel-heading"><div><span className="eyebrow">FORMULARIO</span><h3>{editing ? 'Editar registro' : 'Nuevo registro'}</h3></div><span className="panel-icon">＋</span></div><form onSubmit={save}><div className="form-grid">{current.fields.map(renderField)}</div><div className="form-actions"><button className="btn-primary" type="submit">{editing ? 'Guardar cambios' : 'Crear registro'}</button>{editing && <button className="btn-secondary" type="button" onClick={() => { setEditing(null); setForm(emptyForm(current.fields)) }}>Cancelar</button>}</div></form></section>
            <section className="panel records-panel"><div className="panel-heading records-heading"><div><span className="eyebrow">DIRECTORIO</span><h3>{current.label}</h3></div><span className="panel-icon">☷</span></div><div className="table-tools"><input placeholder="Buscar en los registros..." value={search} onChange={e => setSearch(e.target.value)}/><button className="btn-secondary" onClick={load}>Actualizar</button></div><div className="table-scroll"><table><thead><tr>{current.columns.map(column => <th key={column}>{column.replace(/([A-Z])/g, ' $1').toUpperCase()}</th>)}<th>ACCIONES</th></tr></thead><tbody>{loading ? <tr><td colSpan={current.columns.length + 1} className="empty-cell">Cargando información...</td></tr> : filtered.length === 0 ? <tr><td colSpan={current.columns.length + 1} className="empty-cell">No hay registros para mostrar.</td></tr> : filtered.map(row => <tr key={row.id}>{current.columns.map(column => <td key={column}>{display(row, column)}</td>)}<td className="row-actions"><button onClick={() => startEdit(row)}>Editar</button><button className="delete-action" onClick={() => remove(row)}>Eliminar</button></td></tr>)}</tbody></table></div></section>
          </div>
        </>}
        <footer>Clínica · Sistema de gestión de pacientes <span>Arquitectura N capas · React + ASP.NET Core</span></footer>
      </div>
    </main>
  </div>
}

function Metric({ icon, label, value, color }) {
  return <div className="metric-card"><div className={`metric-icon ${color}`}>{icon}</div><div><span>{label}</span><strong>{value}</strong></div></div>
}

createRoot(document.getElementById('root')).render(<App />)
