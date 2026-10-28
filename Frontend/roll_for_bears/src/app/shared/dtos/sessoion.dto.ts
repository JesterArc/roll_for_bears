import { SessionPlayerDto, SessionRole } from './session-player.dto';

export interface SessionDto {
  id: string;
  name: string;
  gameMode: string;
  description: string;
  playerSlots: string;
  language: string;
  date: string;

  gameSystem?: string;
  creatorRole?: SessionRole;
  players?: SessionPlayerDto[];
}
