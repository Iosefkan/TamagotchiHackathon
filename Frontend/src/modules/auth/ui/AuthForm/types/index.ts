export interface AuthFormStateLogin {
  username: string
  password: string
}

export interface AuthFormStateRegistration {
  username: string
  password: string
  confirmPassword: string,
}

export enum AuthFormType {
  Login = 'login',
  Registration = 'registration',
}
