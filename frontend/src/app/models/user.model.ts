export interface CreateUserDto {
  handle: string;
  email: string;
  password: string;
}

export interface LoginUserDto {
  email: string;
  password: string;
}
