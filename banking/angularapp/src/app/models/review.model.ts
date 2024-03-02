import { User } from "./user.model";

export class Review {
    ReviewId: number;
    userId: number;
    Body: string;
    Rating: number;
    DateCreated: Date;
    user?: User; 
}
