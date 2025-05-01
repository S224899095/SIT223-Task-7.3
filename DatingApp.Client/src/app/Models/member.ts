import { PhotoModel } from "./photo"

export interface MemberModel {
    id: number
    username: string
    photoUrl: string
    age: number
    dateOfBirth: string
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    introduction: string
    lookingFor: string
    interests: string
    city: string
    country: string
    photos: PhotoModel[]
  }
  
