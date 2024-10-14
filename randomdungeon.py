import random

class Wall:
    HOLE = 0
    WALL = 1
    FLOOR = 2
    SPLIT = 3

class DungeonGenerator:
    def __init__(self, width, height):
        self.width = width
        self.height = height
        self.walls = [[Wall.HOLE for _ in range(width)] for _ in range(height)]
        self.max_subdungeon_size = 14
        self.min_room_size = 5

    def generate_dungeon(self):
        self.split_recursive(0, 0, self.width-1, self.height-1)

    def split_recursive(self, x1, y1, x2, y2):
        print(x1, y1, x2, y2)
        x_center = (x1+x2)//2
        y_center = (y1+y2)//2

        width = x2 - x1 + 1
        height = y2 - y1 + 1

        if width < self.max_subdungeon_size and height < self.max_subdungeon_size:
            room_x1 = random.randint(x1, max(x_center - self.min_room_size//2, x1))
            room_y1 = random.randint(y1, max(y_center - self.min_room_size//2, y1))
            room_x2 = random.randint(min(x2, x_center + (self.min_room_size+1)//2 - 1), x2)
            room_y2 = random.randint(min(y2, y_center + (self.min_room_size+1)//2 - 1), y2)
            
            self.create_room(room_x1, room_y1, room_x2, room_y2)
            return

        horizontal_split = random.choice([True, False])
        if width < self.max_subdungeon_size: horizontal_split = True
        if height < self.max_subdungeon_size: horizontal_split = False

        if horizontal_split:
            split_position = random.randint(y1 + self.max_subdungeon_size//2 - 1, y2 - self.max_subdungeon_size//2 + 1)

            self.split_recursive(x1, y1, x2, split_position)
            self.split_recursive(x1, split_position, x2, y2)

            self.create_hallway(x_center, split_position, False)
        else:
            split_position = random.randint(x1 + self.max_subdungeon_size//2 - 1, x2 - self.max_subdungeon_size//2 + 1)

            self.split_recursive(x1, y1, split_position, y2)
            self.split_recursive(split_position, y1, x2, y2)

            self.create_hallway(split_position, y_center, True)

    def create_room(self, x1, y1, x2, y2):
        for i in range(x1 + 1, x2):
            for j in range(y1 + 1, y2):
                self.walls[j][i] = Wall.FLOOR
        for i in range(x1, x2+1):
            self.walls[y1][i] = Wall.WALL
            self.walls[y2][i] = Wall.WALL
        for i in range(y1, y2+1):
            self.walls[i][x1] = Wall.WALL
            self.walls[i][x2] = Wall.WALL

    def place_horizontal(self, x, y):
        self.walls[y+1][x] = Wall.WALL
        self.walls[y][x] = Wall.FLOOR
        self.walls[y-1][x] = Wall.WALL
    
    def place_vertical(self, x, y):
        self.walls[y][x-1] = Wall.WALL
        self.walls[y][x] = Wall.FLOOR
        self.walls[y][x+1] = Wall.WALL

    def create_hallway(self, x, y, is_horizontal):
        if is_horizontal:
            for i in range(x, self.width):
                if self.walls[y][i] == Wall.FLOOR: break
                self.place_horizontal(i, y)
            for i in range(x-1, -1, -1):
                if self.walls[y][i] == Wall.FLOOR: break
                self.place_horizontal(i, y)
        else:
            for i in range(y, self.height):
                if self.walls[i][x] == Wall.FLOOR: break
                self.place_vertical(x, i)
            for i in range(y-1, -1, -1):
                if self.walls[i][x] == Wall.FLOOR: break
                self.place_vertical(x, i)

    def print_dungeon(self):
        for row in self.walls:
            def m(x):
                if x==Wall.HOLE: return "."
                if x==Wall.WALL: return "#"
                if x==Wall.FLOOR: return " "
                if x==Wall.SPLIT: return "/"
            print(" ".join(map(m, row)))

if __name__ == "__main__":
    dungeon_width = 40
    dungeon_height = 20

    generator = DungeonGenerator(dungeon_width, dungeon_height)
    generator.generate_dungeon()
    generator.print_dungeon()
