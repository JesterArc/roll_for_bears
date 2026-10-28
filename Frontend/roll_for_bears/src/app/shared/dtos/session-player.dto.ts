export type SessionRole = 'Player' | 'GM';

export interface SessionPlayerDto {
  id: string;
  name: string;
  role: SessionRole | null;
  is_owner: boolean;
}
