
import { UserProfile } from '../../../core/auth/models/user-profile';

export interface RegisterResponse {
    profile: UserProfile;
    accessToken: string;
}
