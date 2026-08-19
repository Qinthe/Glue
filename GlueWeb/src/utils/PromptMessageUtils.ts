import { i18n } from '@/i18n'

const ERROR_MESSAGE_MAP: Record<string, string> = {
  USER_NOT_EXIST: 'auth.login.userNotExist',
  ACCOUNT_DEACTIVATED: 'auth.login.accountDeactivated',
  INCORRECT_PASSWORD: 'auth.login.incorrectPassword',
  LOGIN_FAILED: 'auth.login.loginFailed',
  ACCOUNT_LOCKED: 'auth.login.accountLocked',

  EMAIL_ALREADY_EXISTS: 'auth.register.emailAlreadyExists',
  USERNAME_ALREADY_EXISTS: 'auth.register.usernameAlreadyExists',
  REGISTRATION_SUCCESS: 'auth.register.registrationSuccess',
  REGISTRATION_FAILED: 'auth.register.registrationFailed',
}

export function resolveResponseErrorMessage(error: unknown) {
  const fallbackKey = 'common.requestFailed'
  const responseData = (error as any)?.response?.data
  const promptTextCode = responseData?.promptTextCode

  if (promptTextCode && ERROR_MESSAGE_MAP[promptTextCode]) {
    let message = i18n.global.t(ERROR_MESSAGE_MAP[promptTextCode], responseData?.data)
    return message
  }

  return responseData?.message || i18n.global.t(fallbackKey)
}