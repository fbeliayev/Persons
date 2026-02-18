export interface City {
  id: number;
  name: string;
  country: string;
}

export interface PersonCity {
  personId: number;
  cityId: number;
  city: City;
  isVisited: boolean;
  visitedDate: string | null;
}
