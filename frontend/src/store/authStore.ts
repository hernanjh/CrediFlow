import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface Usuario {
  id: string
  nombreCompleto: string
  email: string
  rol: 'SUPER_ADMIN' | 'ADMIN' | 'VENDEDOR'
  fotoPerfil?: string
  porcentajeComision: number
}

interface AuthState {
  isAuthenticated: boolean
  usuario: Usuario | null
  accessToken: string | null
  refreshToken: string | null
  login: (accessToken: string, refreshToken: string, usuario: Usuario) => void
  logout: () => void
  updateToken: (accessToken: string, refreshToken: string) => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      isAuthenticated: false,
      usuario: null,
      accessToken: null,
      refreshToken: null,

      login: (accessToken, refreshToken, usuario) =>
        set({ isAuthenticated: true, accessToken, refreshToken, usuario }),

      logout: () =>
        set({ isAuthenticated: false, accessToken: null, refreshToken: null, usuario: null }),

      updateToken: (accessToken, refreshToken) =>
        set({ accessToken, refreshToken }),
    }),
    { name: 'crediflow-auth' }
  )
)
