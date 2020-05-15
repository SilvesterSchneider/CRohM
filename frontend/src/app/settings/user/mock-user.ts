import { UserDto } from '../../shared/api-generated/api-generated';

export const USERS: UserDto[] = [
    {
        id: 1,
        userName: 'admin',
        email: 'admin@mail.com',
        twoFactorEnabled: false,
        userLockEnabled: false
    }
];
