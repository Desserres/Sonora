import mysql.connector

def addContentToDatabase(db, cursor):

    title = input("Enter the title of the music: ")
    image_path = input("Enter the path to the image: ")
    music_path = input("Enter the path to the music file: ")
    favori = int(input("Enter 1 if the music is a favorite, 0 otherwise: "))

    sql = """
    INSERT INTO MusicPlayer (title, image, file, est_favori)
    VALUES (%s, %s, %s, %s)
    """

    values = (title, image_path, music_path, favori)

    cursor.execute(sql, values)

    db.commit()

    print("Music added successfully.")

def main():

    db = mysql.connector.connect(
        host="localhost",
        user="[username]",
        password="[password]",
        database="MusicPlayer"
    )

    cursor = db.cursor()

    addContentToDatabase(db, cursor)

    cursor.close()
    db.close()

if __name__ == "__main__":
    main()
