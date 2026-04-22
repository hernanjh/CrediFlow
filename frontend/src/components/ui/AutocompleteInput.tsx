import { useState, useEffect, useRef } from 'react'

interface AutocompleteOption {
  id: string
  label: string
  subLabel?: string
}

interface AutocompleteInputProps {
  placeholder?: string
  value?: string // ID seleccionado
  onChange: (id: string, option: AutocompleteOption | null) => void
  fetchOptions: (search: string) => Promise<AutocompleteOption[]>
  initialDisplay?: string // Para cuando abrimos en modo edición y queremos mostrar el nombre textual
  className?: string
  required?: boolean
}

export default function AutocompleteInput({ placeholder, onChange, fetchOptions, initialDisplay, className, required }: AutocompleteInputProps) {
  const [query, setQuery] = useState(initialDisplay || '')
  const [options, setOptions] = useState<AutocompleteOption[]>([])
  const [isOpen, setIsOpen] = useState(false)
  const [loading, setLoading] = useState(false)
  const wrapperRef = useRef<HTMLDivElement>(null)

  // Cierre click-afuera
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  // Debounced Search
  useEffect(() => {
    if (!isOpen || query.trim().length === 0) {
      setOptions([])
      return;
    }
    const delayDebounceFn = setTimeout(async () => {
      setLoading(true)
      try {
        const results = await fetchOptions(query)
        setOptions(results)
      } catch (e) {
        console.error("Error buscando:", e)
      } finally {
        setLoading(false)
      }
    }, 400) // 400ms debounce

    return () => clearTimeout(delayDebounceFn)
  }, [query, isOpen])

  const handleSelect = (option: AutocompleteOption) => {
    setQuery(option.label)
    setIsOpen(false)
    onChange(option.id, option)
  }
  
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setQuery(e.target.value)
    setIsOpen(true)
    onChange('', null) // Deseleccionar al modifcar texto manual
  }

  return (
    <div className={`autocomplete-wrapper ${className || ''}`} ref={wrapperRef} style={{ position: 'relative', width: '100%' }}>
      <input
        type="text"
        className="form-input"
        placeholder={placeholder || 'Escriba para buscar...'}
        value={query}
        onChange={handleChange}
        onFocus={() => { if (query) setIsOpen(true) }}
        required={required}
        autoComplete="off"
      />
      {/* Spinner embebido visual */}
      {loading && <div style={{ position: 'absolute', right: 10, top: 12, fontSize: 12, color: 'var(--text-muted)' }}>Cargando...</div>}
      
      {/* Dropdown flotante */}
      {isOpen && query.trim().length > 0 && (
        <ul className="card" style={{ 
            position: 'absolute', top: '100%', left: 0, right: 0, zIndex: 1000, 
            marginTop: 4, padding: 0, maxHeight: 250, overflowY: 'auto', listStyle: 'none'
        }}>
          {!loading && options.length === 0 ? (
            <li style={{ padding: '10px 15px', color: 'var(--text-muted)', fontSize: 13 }}>No se encontraron resultados</li>
          ) : (
            options.map(opt => (
              <li 
                key={opt.id} 
                onClick={() => handleSelect(opt)}
                style={{ 
                  padding: '10px 15px', cursor: 'pointer', borderBottom: '1px solid var(--color-surface-2)',
                  transition: 'background 0.2s', fontSize: 14
                }}
                onMouseEnter={e => (e.currentTarget.style.background = 'var(--color-surface-2)')}
                onMouseLeave={e => (e.currentTarget.style.background = 'transparent')}
              >
                <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{opt.label}</div>
                {opt.subLabel && <div style={{ fontSize: 12, color: 'var(--text-muted)' }}>{opt.subLabel}</div>}
              </li>
            ))
          )}
        </ul>
      )}
    </div>
  )
}
