export class User {
  username: string;
  privileges: UserPrivileges;
  token: string;
  expiration: Date;
  // add what else is needed
}

export enum UserPrivileges {
  User = 0,
  Admin,
  SuperAdmin
}
