
import { UserProfile } from '../../../core/auth/user-profile';

export interface RegisterResponse {
    profile: UserProfile;
    accessToken: string;
}
