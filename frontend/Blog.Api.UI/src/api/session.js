export const tokenKeys = { access: 'nca_access_token', refresh: 'nca_refresh_token' }
export const session = {
  access: localStorage.getItem(tokenKeys.access) || '',
  refresh: localStorage.getItem(tokenKeys.refresh) || '',
  save(tokens) {
    this.access = tokens.accessToken
    this.refresh = tokens.refreshToken
    localStorage.setItem(tokenKeys.access, this.access)
    localStorage.setItem(tokenKeys.refresh, this.refresh)
    window.dispatchEvent(new Event('nca:tokens'))
  },
  clear() {
    this.access = ''
    this.refresh = ''
    localStorage.removeItem(tokenKeys.access)
    localStorage.removeItem(tokenKeys.refresh)
    window.dispatchEvent(new Event('nca:tokens'))
  },
}
